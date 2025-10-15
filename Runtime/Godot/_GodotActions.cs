using Godot;
using System;

namespace LunyScratch
{
	internal sealed class GodotActions : IEngineActions
	{
		public void LogInfo(String message) => GD.Print(message);
		public void LogWarn(String message)
		{
			// Godot 4 provides GD.PushWarning; fall back to Print if unavailable in runtime environment
			GD.PushWarning(message);
		}
		public void LogError(String message) => GD.PushError(message);

		public void ShowMessage(String message, Double duration)
		{
			// Minimal implementation: log to output. UI hookup can be added by the game.
			GD.Print($"[Message] {message} (for {duration:0.###}s)");
		}

		public Double GetDeltaTimeInSeconds() => Engine.GetProcessDeltaTime();
		public Double GetFixedDeltaTimeInSeconds()
		{
			var tps = Engine.PhysicsTicksPerSecond;
			return tps > 0 ? 1.0 / tps : 0.0;
		}
		public Double GetCurrentTimeInSeconds() => Time.GetTicksMsec() / 1000.0;

		public Boolean IsKeyPressed(Key key) => Input.IsKeyPressed(ConvertKey(key));
		public Boolean IsKeyJustPressed(Key key) => Input.IsKeyPressed(ConvertKey(key)); // Godot: use action for edges; fallback to pressed
		public Boolean IsKeyJustReleased(Key key) => false; // Not supported without input mapping; keep simple for now

		public Boolean IsMouseButtonPressed(MouseButton button) => Input.IsMouseButtonPressed(ConvertMouse(button));
		public Boolean IsMouseButtonJustPressed(MouseButton button) => false; // Minimal stub
		public Boolean IsMouseButtonJustReleased(MouseButton button) => false; // Minimal stub

		public IEngineObject InstantiatePrefab(IEnginePrefabAsset prefab, ITransform likeTransform)
		{
			if (prefab is GodotPrefabAsset godotPrefab)
			{
				var sceneTree = Engine.GetMainLoop() as SceneTree;
				var node = godotPrefab.PackedScene.Instantiate();
				if (node is Node n)
				{
					sceneTree?.Root.CallDeferred(Node.MethodName.AddChild, n);
				}
				return new GodotEngineObject(node);
			}
			GameEngine.Actions.LogWarn("InstantiatePrefab: Unsupported prefab asset type.");
			return null;
		}

		public void ReloadCurrentScene()
		{
			var tree = Engine.GetMainLoop() as SceneTree;
			if (tree?.CurrentScene != null)
				tree.ReloadCurrentScene();
		}

		public void QuitApplication()
		{
			var tree = Engine.GetMainLoop() as SceneTree;
			tree?.Quit();
		}

		private static Godot.Key ConvertKey(Key key)
		{
			// Map a subset commonly used; extend as needed
			switch (key)
			{
				case Key.W: return Godot.Key.W;
				case Key.A: return Godot.Key.A;
				case Key.S: return Godot.Key.S;
				case Key.D: return Godot.Key.D;
				case Key.Space: return Godot.Key.Space;
				case Key.LeftArrow: return Godot.Key.Left;
				case Key.RightArrow: return Godot.Key.Right;
				case Key.UpArrow: return Godot.Key.Up;
				case Key.DownArrow: return Godot.Key.Down;
				default: return Godot.Key.None;
			}
		}

  private static Godot.MouseButton ConvertMouse(MouseButton button)
		{
			switch (button)
			{
				case MouseButton.Left: return Godot.MouseButton.Left;
				case MouseButton.Right: return Godot.MouseButton.Right;
				case MouseButton.Middle: return Godot.MouseButton.Middle;
				default: return 0;
			}
		}
	}
}
