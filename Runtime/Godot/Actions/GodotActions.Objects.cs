using Godot;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		public IEngineObject InstantiatePrefab(IEnginePrefabAsset prefab, ITransform transform)
		{
			if (prefab is GodotPrefabAsset godotPrefab)
			{
				var sceneTree = Engine.GetMainLoop() as SceneTree;
				var node = godotPrefab.PackedScene.Instantiate();
				if (node is Node3D node3D)
				{
					var pos = transform.Position;
					node3D.Position = new Vector3(pos.X, pos.Y, pos.Z);
				}

				if (node is Node n)
					sceneTree?.Root.CallDeferred(Node.MethodName.AddChild, n);

				return new GodotEngineObject(node);
			}
			GameEngine.Actions.LogWarn("InstantiatePrefab: Unsupported prefab asset type.");
			return null;
		}
	}
}
