using Godot;
using System;

namespace LunyScratch
{
	/// <summary>
	/// Base class for all Scratch-style behaviors in Godot.
	/// Automatically initializes ScratchRuntime on first use.
	/// </summary>
	[GlobalClass]
	public partial class ScratchNode2D : Node2D, IScratchRunner
	{
		private ScratchRunnerHost _host;
		public Table Variables => _host.Variables;
		public Table GlobalVariables => ScratchRuntime.Singleton.Variables;
		public IEngineHUD HUD { get => ScratchRuntime.Singleton.HUD; set => ScratchRuntime.Singleton.HUD = value; }
		public IEngineMenu Menu { get => ScratchRuntime.Singleton.Menu; set => ScratchRuntime.Singleton.Menu = value; }
		public IEngineCamera ActiveCamera => ScratchRuntime.Singleton.ActiveCamera;

		public ScratchNode2D() => ScratchRuntime.Initialize();

		// IScratchRunner implementation
		public void Run(params IScratchBlock[] blocks) => _host.Run(blocks);
		public void RunPhysics(params IScratchBlock[] blocks) => _host.RunPhysics(blocks);
		public void RepeatForever(params IScratchBlock[] blocks) => Run(Blocks.RepeatForever(blocks));
		public void RepeatForeverPhysics(params IScratchBlock[] blocks) => RunPhysics(Blocks.RepeatForever(blocks));
		public void When(EventBlock evt, params IScratchBlock[] blocks) => Run(Blocks.When(evt, blocks));

		public override void _Ready() => OnScratchReady();

		public override void _EnterTree()
		{
			_host = new ScratchRunnerHost(this, this);
			WireCollisionSignalsRecursive(this);
		}

		public override void _ExitTree() => _host.Dispose();
		public override void _Process(Double deltaTimeInSeconds) => _host.ProcessUpdate(deltaTimeInSeconds);
		public override void _PhysicsProcess(Double delta) => _host.ProcessPhysicsUpdate(delta);

		protected virtual void OnScratchReady() {}

		private void WireCollisionSignalsRecursive(Node root)
		{
			if (root == null)
				return;

			if (root is RigidBody2D r2d)
			{
				var callable = new Callable(this, nameof(OnScratchBodyEntered));
				if (!r2d.IsConnected(RigidBody2D.SignalName.BodyEntered, callable))
					r2d.Connect(RigidBody2D.SignalName.BodyEntered, callable);
			}
			else if (root is Area2D area2d)
			{
				var callable = new Callable(this, nameof(OnScratchBodyEntered));
				if (!area2d.IsConnected(Area2D.SignalName.BodyEntered, callable))
					area2d.Connect(Area2D.SignalName.BodyEntered, callable);
			}

			foreach (var child in root.GetChildren())
			{
				if (child is Node n)
					WireCollisionSignalsRecursive(n);
			}
		}

		private void OnScratchBodyEntered(Node body) => _host?.Context?.EnqueueCollisionEnter(body);
	}
}
