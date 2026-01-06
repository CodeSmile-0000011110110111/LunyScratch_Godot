using Godot;
using System;

namespace LunyScratch
{
	internal struct ScratchVector3 : IVector3
	{
		public Single X { get; set; }
		public Single Y { get; set; }
		public Single Z { get; set; }

		public ScratchVector3(Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		public ScratchVector3(Vector2 v)
		{
			X = v.X;
			Y = v.Y;
			Z = 0f;
		}

		public ScratchVector3(Single x, Single y, Single z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}
