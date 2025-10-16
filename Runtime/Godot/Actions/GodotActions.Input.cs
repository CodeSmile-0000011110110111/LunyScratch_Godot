using Godot;
using System;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		public Boolean IsKeyPressed(Key key) => Input.IsKeyPressed(ConvertKey(key));
		public Boolean IsKeyJustPressed(Key key) => Input.IsKeyPressed(ConvertKey(key)); // Godot: use action for edges; fallback to pressed
		public Boolean IsKeyJustReleased(Key key) => false; // Not supported without input mapping; keep simple for now
		public Boolean IsMouseButtonPressed(MouseButton button) => Input.IsMouseButtonPressed(ConvertMouse(button));
		public Boolean IsMouseButtonJustPressed(MouseButton button) => false; // Minimal stub
		public Boolean IsMouseButtonJustReleased(MouseButton button) => false; // Minimal stub

		private static Godot.Key ConvertKey(Key key)
		{
			// Map a subset commonly used; extend as needed
			switch (key)
			{
				case Key.W:
					return Godot.Key.W;
				case Key.A:
					return Godot.Key.A;
				case Key.S:
					return Godot.Key.S;
				case Key.D:
					return Godot.Key.D;
				case Key.Space:
					return Godot.Key.Space;
				case Key.LeftArrow:
					return Godot.Key.Left;
				case Key.RightArrow:
					return Godot.Key.Right;
				case Key.UpArrow:
					return Godot.Key.Up;
				case Key.DownArrow:
					return Godot.Key.Down;
				default:
					return Godot.Key.None;
			}
		}

		private static Godot.MouseButton ConvertMouse(MouseButton button)
		{
			switch (button)
			{
				case MouseButton.Left:
					return Godot.MouseButton.Left;
				case MouseButton.Right:
					return Godot.MouseButton.Right;
				case MouseButton.Middle:
					return Godot.MouseButton.Middle;
				default:
					return 0;
			}
		}
	}
}
