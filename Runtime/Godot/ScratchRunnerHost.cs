using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LunyScratch
{
	/// <summary>
	/// Internal helper that encapsulates common Scratch runner logic for Godot nodes.
	/// Holds the Table of variables, creates the ScratchBehaviourContext and BlockRunner,
	/// and exposes delegation methods used by ScratchNode, ScratchNode2D and ScratchNode3D.
	/// </summary>
	internal sealed class ScratchRunnerHost
	{
		private readonly IScratchRunner _runnerOwner;
		private readonly Node _hostNode; // Godot node passed as engine object into context
		private readonly Table _variables = new();
		[NotNull] private BlockRunner _runner;
		[NotNull] private ScratchNodeContext _context;

		internal Table Variables => _variables;
		internal ScratchNodeContext Context => _context;

		internal ScratchRunnerHost(IScratchRunner runnerOwner, Node hostNode)
		{
			_runnerOwner = runnerOwner;
			_hostNode = hostNode;
			_context = new ScratchNodeContext(_hostNode, _runnerOwner);
			_runner = new BlockRunner(_context);
		}

		internal void ClearAllBlocks()
		{
			_runner.Clear();
		}

		internal void Dispose() => _runner?.Dispose();

		internal void Run(params IScratchBlock[] blocks)
		{
			if (blocks == null)
				return;

			_runner.AddBlock(Blocks.Sequence(blocks));
		}

		internal void RunPhysics(params IScratchBlock[] blocks)
		{
			if (blocks == null)
				return;

			_runner.AddPhysicsBlock(Blocks.Sequence(blocks));
		}

		internal void ProcessUpdate(Double deltaTimeInSeconds)
		{
			_runner.ProcessUpdate(deltaTimeInSeconds);

			// FIXME: this should be a late pass over all registered nodes, causes: Error calling deferred method on shutdown!
			// var sceneTree = Engine.GetMainLoop() as SceneTree;
			// sceneTree?.Root.CallDeferred(nameof(ClearCollisionEventQueues));
			_context?.ClearCollisionEventQueues();
		}

		// private void ClearCollisionEventQueues() => _context?.ClearCollisionEventQueues();

		internal void ProcessPhysicsUpdate(Double deltaTimeInSeconds) => _runner.ProcessPhysicsUpdate(deltaTimeInSeconds);
	}
}
