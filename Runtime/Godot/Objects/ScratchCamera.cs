using Godot;

namespace LunyScratch
{
	public sealed class ScratchCamera : IEngineCamera
	{
		private readonly GodotObject _camera;

		public ScratchCamera(GodotObject camera) => _camera = camera;

		public void SetTrackingTarget(IEngineObject target)
		{
			// Godot cameras typically do not have a built-in tracking target like Cinemachine.
			// Implementing follow would require a custom script; for now, this is a no-op.
		}
	}
}
