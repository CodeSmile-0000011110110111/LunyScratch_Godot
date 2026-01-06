using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace LunyScratch
{
	internal sealed class GodotAssetRegistry : AssetRegistry.IAssetRegistry
	{
		private readonly Dictionary<String, IEngineAsset> _cache = new();

		public T Get<T>(String path) where T : class, IEngineAsset
		{
			var asset = Get(path, typeof(T));
			return asset as T;
		}

		public IEngineAsset Get(String path, Type assetType)
		{
			if (String.IsNullOrWhiteSpace(path))
				return GetPlaceholder(assetType);

			if (_cache.TryGetValue(path, out var cached))
				return cached;

			var assetPath = path.StartsWith("res://") ? path : $"res://{path}";
			if (Path.GetExtension(assetPath) == String.Empty)
				assetPath = $"{assetPath}.tscn"; // assume scene

			var res = GD.Load(assetPath);
			IEngineAsset wrapped = null;
			if (res is PackedScene ps && typeof(IEnginePrefabAsset).IsAssignableFrom(assetType))
				wrapped = new GodotPrefabAsset(ps);
			// Extend here for UI/audio assets if needed

			if (wrapped == null)
				return GetPlaceholder(assetType);

			_cache[path] = wrapped;
			return wrapped;
		}

		public T GetPlaceholder<T>() where T : class, IEngineAsset => GetPlaceholder(typeof(T)) as T;

		public IEngineAsset GetPlaceholder(Type assetType)
		{
			if (typeof(IEnginePrefabAsset).IsAssignableFrom(assetType))
				return GodotPrefabAsset.CreatePlaceholder();

			// Default: return null placeholder for unknown types
			return null;
		}
	}
}
