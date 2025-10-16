using System;
using Godot;

namespace LunyScratch
{
	internal struct ScratchVector3 : IVector3
	{
		public Single X { get; set; }
		public Single Y { get; set; }
		public Single Z { get; set; }

		public ScratchVector3(Vector3 v)
		{
			X = (Single)v.X;
			Y = (Single)v.Y;
			Z = (Single)v.Z;
		}

		public ScratchVector3(Vector2 v)
		{
			X = (Single)v.X;
			Y = (Single)v.Y;
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
