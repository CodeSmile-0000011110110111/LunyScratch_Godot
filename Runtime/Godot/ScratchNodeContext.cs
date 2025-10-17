using Godot;
using System;
using System.Collections.Generic;

namespace LunyScratch
{
	/// <summary>
	/// Minimal global context for blocks not bound to a specific node.
	/// Provides access to the runner and basic engine operations. Component-specific properties return null.
	/// </summary>
	internal sealed class ScratchNodeContext : IScratchContext
	{
		private readonly Node _host;
		private readonly IScratchRunner _runner;
		private readonly Dictionary<String, IEngineObject> _childCache = new();
		private readonly HashSet<Node> _collisionEnterQueue = new();

		private IRigidbody _cachedRigidbody;
		private ITransform _cachedTransform;
		private IEngineAudioSource _cachedAudio;
		private IEngineCamera _cachedCamera;
		private readonly Dictionary<Type, IEngineObject> _childTypeCache = new();

		public IRigidbody Rigidbody
		{
			get
			{
				if (_cachedRigidbody != null)
					return _cachedRigidbody;

				if (_host is Node2D || _host is Node3D)
				{
					_cachedRigidbody = GetOrCreateRigidbodyAdapter();
					return _cachedRigidbody;
				}
				throw new InvalidOperationException("ScratchNodeContext host must be a Node2D or Node3D to access Rigidbody.");
			}
		}

		public ITransform Transform
		{
			get
			{
				if (_cachedTransform != null)
					return _cachedTransform;

				if (_host is Node2D || _host is Node3D)
				{
					_cachedTransform = new ScratchTransform(_host);
					return _cachedTransform;
				}
				throw new InvalidOperationException("ScratchNodeContext host must be a Node2D or Node3D to access Transform.");
			}
		}
		public IEngineAudioSource Audio
		{
			get
			{
				if (_cachedAudio != null)
					return _cachedAudio;
				if (_host == null)
					return null;

				var found = FindNodeByTypes(_host, typeof(AudioStreamPlayer), typeof(AudioStreamPlayer2D), typeof(AudioStreamPlayer3D));
				if (found != null)
					_cachedAudio = new ScratchAudioSource(found);
				return _cachedAudio;
			}
		}
		public IEngineObject Self => _host != null ? new GodotEngineObject(_host) : null;
		public IScratchRunner Runner => _runner;
		public IEngineCamera ActiveCamera
		{
			get
			{
				if (_cachedCamera != null)
					return _cachedCamera;
				if (_host == null)
					return null;

				var found = FindNodeByTypes(_host, typeof(Camera3D), typeof(Camera2D));
				if (found != null)
					_cachedCamera = new ScratchCamera(found);
				return _cachedCamera;
			}
		}

		public ScratchNodeContext(Node host, IScratchRunner runner)
		{
			_host = host;
			_runner = runner;
		}

		public void SetSelfComponentEnabled(Boolean enabled)
		{
			if (_host != null)
				_host.ProcessMode = enabled ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
		}

		public IEngineHUD GetEngineHUD() => ScratchRuntime.Singleton.HUD;
		public IEngineMenu GetEngineMenu() => ScratchRuntime.Singleton.Menu;

		public void ScheduleDestroy()
		{
			if (_host != null)
				_host.QueueFree();
		}

		public IEngineObject FindChild(String name)
		{
			if (_host == null || String.IsNullOrEmpty(name))
				return null;
			if (_childCache.TryGetValue(name, out var cached))
				return cached;

			var found = _host.FindChild(name, true, false) as GodotObject;
			if (found != null)
			{
				var wrapped = new GodotEngineObject(found);
				_childCache[name] = wrapped;
				return wrapped;
			}
			return null;
		}

		public IEngineObject FindChild(Type type)
		{
			if (_host == null || type == null)
				return null;
			if (_childTypeCache.TryGetValue(type, out var cached))
				return cached;

			var found = FindFirstByType(_host, type);
			if (found != null)
			{
				var wrapped = new GodotEngineObject(found);
				_childTypeCache[type] = wrapped;
				return wrapped;
			}
			return null;
		}

		private static GodotObject FindNodeByTypes(Node start, params Type[] types)
		{
			if (start == null || types == null || types.Length == 0)
				return null;

			// Check self first
			foreach (var t in types)
			{
				if (t.IsInstanceOfType(start))
					return start;
			}

			// DFS on children
			foreach (var child in start.GetChildren())
			{
				var cnode = child as Node;
				if (cnode == null) continue;
				foreach (var t in types)
				{
					if (t.IsInstanceOfType(cnode))
						return cnode;
				}
				var deeper = FindNodeByTypes(cnode, types);
				if (deeper != null)
					return deeper;
			}
			return null;
		}

		private static GodotObject FindFirstByType(Node start, Type type)
		{
			if (start == null || type == null)
				return null;
			if (type.IsInstanceOfType(start))
				return start;
			foreach (var child in start.GetChildren())
			{
				if (child is Node node)
				{
					if (type.IsInstanceOfType(node))
						return node;
					var deeper = FindFirstByType(node, type);
					if (deeper != null)
						return deeper;
				}
			}
			return null;
		}

		public Boolean QueryCollisionEnterEvents(String nameFilter, String tagFilter)
		{
			foreach (var other in _collisionEnterQueue)
			{
				if (other == null)
					continue;
				var nameOk = nameFilter == null || String.Equals(other.Name, nameFilter, StringComparison.InvariantCulture);
				var tagOk = tagFilter == null || other.IsInGroup(tagFilter);
				if (nameOk && tagOk)
					return true;
			}
			return false;
		}

		internal void EnqueueCollisionEnter(Node other)
		{
			if (other != null)
			{
				_collisionEnterQueue.Add(other);
			}
		}

		internal void ClearCollisionEventQueues() => _collisionEnterQueue.Clear();

		private void TryConnectBodyEntered(Node bodyNode)
		{
			if (_host == null || bodyNode == null)
				return;

			if (!_host.HasMethod("OnScratchBodyEntered"))
				return;

			var callable = new Callable(_host, "OnScratchBodyEntered");

			// Use typed signals for both 2D and 3D
			if (bodyNode is RigidBody2D r2d)
			{
				var sig = RigidBody2D.SignalName.BodyEntered;
				if (!r2d.IsConnected(sig, callable))
					r2d.Connect(sig, callable);
			}
			else if (bodyNode is Area2D area2d)
			{
				var sig = Area2D.SignalName.BodyEntered;
				if (!area2d.IsConnected(sig, callable))
					area2d.Connect(sig, callable);
			}
			else if (bodyNode is RigidBody3D r3d)
			{
				var sig = RigidBody3D.SignalName.BodyEntered;
				if (!r3d.IsConnected(sig, callable))
					r3d.Connect(sig, callable);
			}
			else if (bodyNode is Area3D area3d)
			{
				var sig = Area3D.SignalName.BodyEntered;
				if (!area3d.IsConnected(sig, callable))
					area3d.Connect(sig, callable);
			}
		}

		private IRigidbody GetOrCreateRigidbodyAdapter()
		{
			if (_host is Node2D n2)
			{
				if (_host is RigidBody2D r2)
				{
					TryConnectBodyEntered(r2);
					return new ScratchRigidbody(r2);
				}

				// search direct children for RigidBody2D
				foreach (var child in n2.GetChildren())
				{
					if (child is RigidBody2D rb2)
					{
						TryConnectBodyEntered(rb2);
						return new ScratchRigidbody(rb2);
					}
				}

				// not found: create one
				var newRb2 = new RigidBody2D { Name = "Rigidbody2D" };
				n2.AddChild(newRb2);
				TryConnectBodyEntered(newRb2);
				return new ScratchRigidbody(newRb2);
			}

			if (_host is Node3D n3)
			{
				if (_host is RigidBody3D r3)
				{
					TryConnectBodyEntered(r3);
					return new ScratchRigidbody(r3);
				}

				foreach (var child in n3.GetChildren())
				{
						if (child is RigidBody3D rb3)
						{
							TryConnectBodyEntered(rb3);
							return new ScratchRigidbody(rb3);
						}
				}

				var newRb3 = new RigidBody3D { Name = "Rigidbody3D" };
				n3.AddChild(newRb3);
				TryConnectBodyEntered(newRb3);
				return new ScratchRigidbody(newRb3);
			}

			throw new InvalidOperationException("ScratchNodeContext host must be a Node2D or Node3D to access Rigidbody.");
		}
	}
}
