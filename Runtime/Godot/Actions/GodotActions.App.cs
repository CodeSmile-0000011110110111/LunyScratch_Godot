using Godot;

namespace LunyScratch
{
	internal sealed partial class GodotActions : IEngineActions
	{
		public void ReloadCurrentScene()
		{
			var tree = ScratchRuntime.SceneTree;
			if (tree?.CurrentScene != null)
			{
				ScratchRuntime.Singleton.OnCurrentSceneUnloading(tree.CurrentScene);

				var error = tree.ReloadCurrentScene();
				if (error != null && error != Error.Ok)
					GD.PrintErr($"ReloadCurrentScene: {error}");
			}
		}

		public void QuitApplication()
		{
			var tree = ScratchRuntime.SceneTree;
			tree?.Quit();
		}
	}
}
