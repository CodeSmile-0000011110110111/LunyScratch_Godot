using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

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
		private IEngineHUD _hud;
		private IEngineMenu _menu;
		[NotNull] private ScratchRunnerHost _host;

		private Node _currentSceneRoot;

		[NotNull] public static ScratchRuntime Singleton => s_Instance;
		[NotNull] public Table GlobalVariables => _host.Variables;

		public IEngineHUD HUD
		{
			get
			{
				if (_hud == null)
					TryFindAtSceneRoot("Hud", out _hud);
				return _hud;
			}
			set => _hud = value;
		}
		public IEngineMenu Menu
		{
			get
			{
				if (_menu == null)
					TryFindAtSceneRoot("Menu", out _menu);
				return _menu;
			}
			set => _menu = value;
		}
		// TODO: implement a Godot camera adapter; return null for now
		public IEngineCamera ActiveCamera => throw new NotImplementedException();
		public Table Variables => _host?.Variables;

		internal static SceneTree SceneTree => Engine.GetMainLoop() as SceneTree;
		internal static Node CurrentSceneRoot => SceneTree.CurrentScene;

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

		public ScratchRuntime() => _host = new ScratchRunnerHost(this, this);

		// IScratchRunner implementation
		public void Run(params IScratchBlock[] blocks) => _host.Run(blocks);
		public void RunPhysics(params IScratchBlock[] blocks) => _host.RunPhysics(blocks);
		public void RepeatForever(params IScratchBlock[] blocks) => Run(Blocks.RepeatForever(blocks));
		public void RepeatForeverPhysics(params IScratchBlock[] blocks) => RunPhysics(Blocks.RepeatForever(blocks));
		public void When(EventBlock evt, params IScratchBlock[] blocks) => Run(Blocks.When(evt, blocks));

		internal void OnCurrentSceneUnloading(Node sceneRoot)
		{
			GD.Print($"ScratchRuntime: OnCurrentSceneUnloading => {sceneRoot?.Name}");
			_host.ClearAllBlocks();
			_hud = null;
			_menu = null;
		}

		private Boolean TryFindAtSceneRoot<T>(String childName, out T result) where T : class
		{
			result = null;

			var sceneRoot = CurrentSceneRoot;
			if (sceneRoot == null)
			{
				GD.PrintErr("ScratchRuntime: scene root not found");
				return false;
			}

			// Expect direct children named accordingly at the root of the scene
			var node = sceneRoot.GetNodeOrNull<Node>(childName);
			if (node == null)
			{
				GD.PrintErr($"ScratchRuntime: UI node '{childName}' not found at scene root {sceneRoot}.");
				return false;
			}

			if (node is T t)
				result = t;
			else
			{
				GD.PrintErr(
					$"ScratchRuntime: Node '{childName}' found but does not implement {typeof(T).Name} (name={node.Name}, type={node.GetType().Name}).");
			}

			return result != null;
		}

		public override void _ExitTree()
		{
			GD.Print("ScratchRuntime: ExitTree");
			_host.Dispose();
			_host = null;
		}

		public override void _Process(Double deltaTimeInSeconds) => _host.ProcessUpdate(deltaTimeInSeconds);
		public override void _PhysicsProcess(Double delta) => _host.ProcessPhysicsUpdate(delta);
	}
}
