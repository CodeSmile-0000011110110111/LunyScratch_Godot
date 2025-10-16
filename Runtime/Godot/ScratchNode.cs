using Godot;
using System;

namespace LunyScratch
{
	/// <summary>
	/// Base class for all Scratch-style behaviors in Godot.
	/// Automatically initializes ScratchRuntime on first use.
	/// </summary>
	public partial class ScratchNode : Node, IScratchRunner
	{
		private readonly Table _variables = new();
		private BlockRunner _runner;
		private ScratchBehaviourContext _context;
		public Table Variables => _variables;

		// IScratchRunner implementation
		public void Run(params IScratchBlock[] blocks)
		{
			if (blocks == null)
				return;

			foreach (var b in blocks)
				_runner.AddBlock(b);
		}

		public void RunPhysics(params IScratchBlock[] blocks)
		{
			if (blocks == null)
				return;

			foreach (var b in blocks)
				_runner.AddPhysicsBlock(b);
		}

		public void RepeatForever(params IScratchBlock[] blocks) => Run(Blocks.RepeatForever(blocks));
		public void RepeatForeverPhysics(params IScratchBlock[] blocks) => RunPhysics(Blocks.RepeatForever(blocks));
		public void When(EventBlock evt, params IScratchBlock[] blocks) => Run(Blocks.When(evt, blocks));
		public override void _Ready() => ScratchRuntime.Initialize();

		public override void _EnterTree()
		{
			_context = new ScratchBehaviourContext(this, this);
			_runner = new BlockRunner(_context);
		}

		public override void _Process(Double deltaTimeInSeconds) => _runner?.ProcessUpdate(deltaTimeInSeconds);

		public override void _PhysicsProcess(Double delta) => _runner?.ProcessPhysicsUpdate(delta);

		public override void _ExitTree() => _runner?.Dispose();
	}
}
