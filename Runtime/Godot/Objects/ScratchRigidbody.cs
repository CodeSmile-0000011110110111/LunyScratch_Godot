using Godot;
using System;

namespace LunyScratch
{
	internal sealed class ScratchRigidbody : IRigidbody
	{
		private readonly RigidBody2D _rb2D;
		private readonly RigidBody3D _rb3D;
		private readonly Node _owner;

		public IVector3 LinearVelocity
		{
			get
			{
				if (_rb3D != null)
					return new ScratchVector3(_rb3D.LinearVelocity);
				if (_rb2D != null)
					return new ScratchVector3(_rb2D.LinearVelocity);

				return new ScratchVector3(0f, 0f, 0f);
			}
		}

		public IVector3 AngularVelocity
		{
			get
			{
				if (_rb3D != null)
					return new ScratchVector3(_rb3D.AngularVelocity);
				if (_rb2D != null)
					return new ScratchVector3(0f, 0f, _rb2D.AngularVelocity);

				return new ScratchVector3(0f, 0f, 0f);
			}
		}

		public IVector3 Position
		{
			get
			{
				if (_rb3D != null)
					return new ScratchVector3(_rb3D.GlobalPosition);
				if (_rb2D != null)
					return new ScratchVector3(_rb2D.GlobalPosition);

				// fallback to owner transform
				if (_owner is Node3D n3)
					return new ScratchVector3(n3.GlobalPosition);
				if (_owner is Node2D n2)
					return new ScratchVector3(n2.GlobalPosition);

				return new ScratchVector3(0f, 0f, 0f);
			}
		}

		public IVector3 Forward
		{
			get
			{
				if (_rb3D != null)
				{
					var fwd = _rb3D.GlobalTransform.Basis.Z;
					return new ScratchVector3(fwd);
				}
				return new ScratchVector3(0f, 0f, 1f);
			}
		}

		public ScratchRigidbody(RigidBody2D rb)
		{
			_rb2D = rb;
			_owner = rb;
		}

		public ScratchRigidbody(RigidBody3D rb)
		{
			_rb3D = rb;
			_owner = rb;
		}

		public void SetLinearVelocity(Single x, Single y, Single z)
		{
			if (_rb3D != null)
				_rb3D.LinearVelocity = new Vector3(x, y, z);
			else if (_rb2D != null)
				_rb2D.LinearVelocity = new Vector2(x, y);
		}

		public void SetAngularVelocity(Single x, Single y, Single z)
		{
			if (_rb3D != null)
				_rb3D.AngularVelocity = new Vector3(x, -y, z);
			else if (_rb2D != null)
				_rb2D.AngularVelocity = z;
		}

		public void SetPosition(Single x, Single y, Single z)
		{
			if (_rb3D != null)
				_rb3D.GlobalPosition = new Vector3(x, y, z);
			else if (_rb2D != null)
				_rb2D.GlobalPosition = new Vector2(x, y);
		}
	}
}
