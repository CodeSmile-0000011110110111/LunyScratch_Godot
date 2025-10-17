using Godot;

namespace LunyScratch
{
	internal sealed partial class GodotActions : IEngineActions
	{
		public void ReloadCurrentScene()
		{
			var tree = ScratchRuntime.GetSceneTree();
			if (tree?.CurrentScene != null)
			{
				tree.ReloadCurrentScene();
			}
		}

		public void QuitApplication()
		{
			var tree = ScratchRuntime.GetSceneTree();
			tree?.Quit();
		}
	}
}
