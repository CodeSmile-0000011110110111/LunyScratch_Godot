using Godot;
using System;

namespace LunyScratch
{
	/// <summary>
	/// Godot runtime singleton that manages step execution.
	/// Auto-initializes when first accessed.
	/// Implements IScratchRunner to provide global block execution.
	/// </summary>
	public sealed partial class ScratchRuntime : Node, IEngineRuntime
	{
		private static ScratchRuntime s_Instance;

		private BlockRunner _runner;
		private GodotGlobalContext _context;
		private readonly Table _variables = new();

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

		public override void _EnterTree()
		{
			_context = new GodotGlobalContext(this, this);
			_runner = new BlockRunner(_context);
		}

		public override void _Process(Double deltaTimeInSeconds)
		{
			_runner?.ProcessUpdate(deltaTimeInSeconds);
		}

		public override void _PhysicsProcess(Double delta)
		{
			_runner?.ProcessPhysicsUpdate(delta);
		}

		public override void _ExitTree()
		{
			_runner?.Dispose();
			GameEngine.Shutdown();
			s_Instance = null;
		}

		// IScratchRunner implementation
		public void Run(params IScratchBlock[] blocks)
		{
			if (blocks == null) return;
			foreach (var b in blocks) _runner.AddBlock(b);
		}

		public void RunPhysics(params IScratchBlock[] blocks)
		{
			if (blocks == null) return;
			foreach (var b in blocks) _runner.AddPhysicsBlock(b);
		}

		public void RepeatForever(params IScratchBlock[] blocks) => Run(Blocks.RepeatForever(blocks));
		public void RepeatForeverPhysics(params IScratchBlock[] blocks) => RunPhysics(Blocks.RepeatForever(blocks));
		public void When(EventBlock evt, params IScratchBlock[] blocks) => Run(Blocks.When(evt, blocks));
		public Table Variables => _variables;
	}
}
