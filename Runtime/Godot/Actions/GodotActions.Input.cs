using Godot;
using System;

namespace LunyScratch
{
	internal sealed partial class GodotActions
	{
		public Boolean IsKeyPressed(Key key) => key == Key.Any ? Input.IsAnythingPressed() : Input.IsPhysicalKeyPressed(Remap.ToGodotKey(key));

		public Boolean IsKeyJustPressed(Key key) => key == Key.Any ? Input.IsAnythingPressed() : Input.IsPhysicalKeyPressed(Remap.ToGodotKey(key));

		// Not supported without input mapping; keep simple for now
		public Boolean IsKeyJustReleased(Key key) => throw new NotImplementedException();

		public Boolean IsMouseButtonPressed(MouseButton button) => Input.IsMouseButtonPressed(Remap.ToGodotMouseButton(button));
		public Boolean IsMouseButtonJustPressed(MouseButton button) => throw new NotImplementedException();
		public Boolean IsMouseButtonJustReleased(MouseButton button) => throw new NotImplementedException();
	}
}
