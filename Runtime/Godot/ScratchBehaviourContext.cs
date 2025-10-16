using Godot;
using System;
using System.Collections.Generic;

namespace LunyScratch
{
	/// <summary>
	/// Minimal global context for blocks not bound to a specific node.
	/// Provides access to the runner and basic engine operations. Component-specific properties return null.
	/// </summary>
	internal sealed class ScratchBehaviourContext : IScratchContext
	{
		private readonly Node _host;
		private readonly IScratchRunner _runner;
		private readonly Dictionary<string, IEngineObject> _childCache = new();

		public ScratchBehaviourContext(Node host, IScratchRunner runner)
		{
			_host = host;
			_runner = runner;
		}

		public IRigidbody Rigidbody => null;
		public ITransform Transform => null;
		public IEngineAudioSource Audio => null;
		public IEngineObject Self => _host != null ? new GodotEngineObject(_host) : null;
		public IScratchRunner Runner => _runner;
		public IEngineCamera ActiveCamera => null;

		public void SetSelfComponentEnabled(Boolean enabled)
		{
			if (_host != null)
			{
				_host.ProcessMode = enabled ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
			}
		}

		public IEngineHUD GetEngineHUD() => null;
		public IEngineMenu GetEngineMenu() => null;
		public void ScheduleDestroy() { if (_host != null) _host.QueueFree(); }

		public IEngineObject FindChild(String name)
		{
			if (_host == null || string.IsNullOrEmpty(name)) return null;
			if (_childCache.TryGetValue(name, out var cached)) return cached;
			var found = _host.FindChild(name, true, false) as GodotObject;
			if (found != null)
			{
				var wrapped = new GodotEngineObject(found);
				_childCache[name] = wrapped;
				return wrapped;
			}
			return null;
		}

		public Boolean QueryCollisionEnterEvents(String nameFilter, String tagFilter) => false;
	}
}
