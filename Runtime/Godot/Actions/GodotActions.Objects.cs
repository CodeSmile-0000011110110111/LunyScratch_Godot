using Godot;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		public IEngineObject InstantiatePrefab(IEnginePrefabAsset prefab, ITransform likeTransform)
		{
			if (prefab is GodotPrefabAsset godotPrefab)
			{
				var sceneTree = Engine.GetMainLoop() as SceneTree;
				var node = godotPrefab.PackedScene.Instantiate();
				if (node is Node n)
				{
					sceneTree?.Root.CallDeferred(Node.MethodName.AddChild, n);
				}
				return new GodotEngineObject(node);
			}
			GameEngine.Actions.LogWarn("InstantiatePrefab: Unsupported prefab asset type.");
			return null;
		}
	}
}
