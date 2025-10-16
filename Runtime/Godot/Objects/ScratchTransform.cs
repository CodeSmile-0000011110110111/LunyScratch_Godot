using Godot;
using System;

namespace LunyScratch
{
	internal sealed class ScratchTransform : ITransform
	{
		private readonly Node _node;

		public IVector3 Position
		{
			get
			{
				if (_node is Node3D n3)
					return new ScratchVector3(n3.GlobalPosition);
				if (_node is Node2D n2)
					return new ScratchVector3(n2.GlobalPosition);

				return new ScratchVector3(0f, 0f, 0f);
			}
		}

		public IVector3 Forward
		{
			get
			{
				if (_node is Node3D n3)
				{
					// Godot forward is -Basis.Z
					var fwd = -n3.GlobalTransform.Basis.Z;
					return new ScratchVector3(fwd);
				}
				// 2D: return +Z forward
				return new ScratchVector3(0f, 0f, 1f);
			}
		}

		public ScratchTransform(Node node) => _node = node;

		public void SetPosition(Single x, Single y, Single z)
		{
			if (_node is Node3D n3)
				n3.GlobalPosition = new Vector3(x, y, z);
			else if (_node is Node2D n2)
				n2.GlobalPosition = new Vector2(x, y);
		}
	}
}
