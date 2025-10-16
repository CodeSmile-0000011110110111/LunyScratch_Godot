using Godot;
using System;
using System.Collections.Generic;
using System.Net;

namespace LunyScratch
{
	internal sealed class GodotAssetRegistry : AssetRegistry.IAssetRegistry
	{
		private readonly Dictionary<string, IEngineAsset> _cache = new();

		public T Get<T>(string path) where T : class, IEngineAsset
		{
			var asset = Get(path, typeof(T));
			return asset as T;
		}

		public IEngineAsset Get(string path, Type assetType)
		{
			if (string.IsNullOrWhiteSpace(path))
				return GetPlaceholder(assetType);

			if (_cache.TryGetValue(path, out var cached))
				return cached;

			var assetPath = path.StartsWith("res://") ? path : $"res://{path}";
			if (System.IO.Path.GetExtension(assetPath) == string.Empty)
				assetPath = $"{assetPath}.tscn"; // assume scene

			Resource res = GD.Load(assetPath);
			IEngineAsset wrapped = null;
			if (res is PackedScene ps && typeof(IEnginePrefabAsset).IsAssignableFrom(assetType))
			{
				wrapped = new GodotPrefabAsset(ps);
			}
			// Extend here for UI/audio assets if needed

			if (wrapped == null)
			{
				return GetPlaceholder(assetType);
			}

			_cache[path] = wrapped;
			return wrapped;
		}

		public T GetPlaceholder<T>() where T : class, IEngineAsset
		{
			return GetPlaceholder(typeof(T)) as T;
		}

		public IEngineAsset GetPlaceholder(Type assetType)
		{
			if (typeof(IEnginePrefabAsset).IsAssignableFrom(assetType))
			{
				return GodotPrefabAsset.CreatePlaceholder();
			}
			// Default: return null placeholder for unknown types
			return null;
		}
	}
}
