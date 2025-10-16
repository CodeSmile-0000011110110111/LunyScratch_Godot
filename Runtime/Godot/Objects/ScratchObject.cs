using Godot;
using System;

namespace LunyScratch
{
	internal sealed class GodotEngineObject : IEngineObject
	{
		private readonly GodotObject _engineObject;

		public static implicit operator GodotEngineObject(GodotObject engineObject) => new(engineObject);

		public GodotEngineObject(GodotObject engineObject) => _engineObject = engineObject;

		public void SetEnabled(Boolean enabled)
		{
			switch (_engineObject)
			{
				case CanvasItem canvasItem: // also catches Node2D
					canvasItem.Visible = enabled;
					break;
				case Node3D node3d:
					node3d.Visible = enabled;
					break;
			}

			if (_engineObject is Node node)
				node.ProcessMode = enabled ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
		}
	}
}
