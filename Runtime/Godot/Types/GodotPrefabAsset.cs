using Godot;

namespace LunyScratch
{
	internal sealed class GodotPrefabAsset : IEnginePrefabAsset
	{
		public PackedScene PackedScene { get; }

		public GodotPrefabAsset(PackedScene scene)
		{
			PackedScene = scene;
		}

		public static GodotPrefabAsset CreatePlaceholder()
		{
			var scene = new PackedScene();
			var root = new Node { Name = "PlaceholderPrefab" };
			var result = scene.Pack(root);
			return new GodotPrefabAsset(scene);
		}
	}
}
