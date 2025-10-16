using Godot;
using System;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		public void ShowMessage(String message, Double duration)
		{
			// Minimal implementation: log to output. UI hookup can be added by the game.
			GD.Print($"[Message] {message} (for {duration:0.###}s)");
		}
	}
}
