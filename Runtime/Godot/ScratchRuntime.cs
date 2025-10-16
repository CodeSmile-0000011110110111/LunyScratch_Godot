using Godot;

namespace LunyScratch
{
	/// <summary>
	/// Godot runtime singleton that manages step execution.
	/// Auto-initializes when first accessed.
	/// Implements IScratchRunner to provide global block execution.
	/// </summary>
	public sealed partial class ScratchRuntime : ScratchNode, IEngineRuntime
	{
		private static ScratchRuntime s_Instance;

		public static ScratchRuntime Instance => s_Instance;

		internal static void Initialize()
		{
			if (s_Instance != null)
				return;

			// Create runtime node and add to scene tree
			s_Instance = new ScratchRuntime();
			s_Instance.Name = nameof(ScratchRuntime);

			// Add to the root to persist across scenes
			var root = Engine.GetMainLoop() as SceneTree;
			root?.Root.CallDeferred(Node.MethodName.AddChild, s_Instance);

			// Initialize the engine abstraction
			GameEngine.Initialize(s_Instance, new GodotActions(), new GodotAssetRegistry());
		}
	}
}
