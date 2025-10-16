using Godot;
using System;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		public void LogInfo(String message) => GD.Print(message);

		public void LogWarn(String message) => GD.PushWarning(message);

		public void LogError(String message) => GD.PushError(message);
	}
}
