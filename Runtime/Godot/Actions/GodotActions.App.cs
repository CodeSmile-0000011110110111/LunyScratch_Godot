using Godot;

namespace LunyScratch
{
	internal sealed partial class GodotActions : IEngineActions
	{
		public void ReloadCurrentScene()
		{
			var tree = Engine.GetMainLoop() as SceneTree;
			if (tree?.CurrentScene != null)
				tree.ReloadCurrentScene();
		}

		public void QuitApplication()
		{
			var tree = Engine.GetMainLoop() as SceneTree;
			tree?.Quit();
		}
	}
}
