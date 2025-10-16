using Godot;
using System;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		private static Double _lastProcessTimeSeconds;

		public Double GetDeltaTimeInSeconds()
		{
			var now = GetCurrentTimeInSeconds();
			// FIXME: this will modify _lastProcessTimeSeconds every call, the next call will have the wrong deltatime!
			// this should get the deltaTime from IScratchRunner instead
			var dt = _lastProcessTimeSeconds > 0 ? now - _lastProcessTimeSeconds : 0.0;
			_lastProcessTimeSeconds = now;
			return dt;
		}

		public Double GetFixedDeltaTimeInSeconds()
		{
			var tps = Engine.PhysicsTicksPerSecond;
			return tps > 0 ? 1.0 / tps : 0.0;
		}

		public Double GetCurrentTimeInSeconds() => Time.GetTicksMsec() / 1000.0;
	}
}
