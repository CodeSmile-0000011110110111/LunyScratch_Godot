using Godot;
using System;
using System.Collections.Generic;

namespace LunyScratch
{
	/// <summary>
	/// Godot implementation of ScratchMenu. Finds all Button descendants of the root Control
	/// and forwards their Pressed events with the button's Name.
	/// </summary>
	[GlobalClass]
	public sealed partial class ScratchMenu : ScratchUI, IEngineMenu
	{
		public event Action<string> OnButtonClicked;

		private readonly Dictionary<Button, Action> _handlers = new();

		public override void _Ready()
		{
			base._Ready();
			ConnectButtonSignals();
		}

		public override void _ExitTree()
		{
			DisconnectButtonSignals();
			base._ExitTree();
		}

		private void ConnectButtonSignals()
		{
			var root = GetRootControl();
			if (root == null)
				return;

			foreach (var btn in FindButtons(root))
			{
				if (_handlers.ContainsKey(btn))
					continue;
				Action handler = () => OnButtonPressed(btn);
				_handlers[btn] = handler;
				btn.Pressed += handler;
			}
		}

		private void DisconnectButtonSignals()
		{
			foreach (var kv in _handlers)
			{
				var btn = kv.Key;
				var handler = kv.Value;
				if (IsInstanceValid(btn))
					btn.Pressed -= handler;
			}
			_handlers.Clear();
		}

		private void OnButtonPressed(Button button)
		{
			GD.Print($"pressed {button?.Name}");
			var name = button?.Name ?? string.Empty;
			if (!string.IsNullOrEmpty(name))
				OnButtonClicked?.Invoke(name);
		}

		private Control GetRootControl()
		{
			if (this is Control c)
				return c;
			return GetNodeOrNull<Control>(".");
		}

		private static IEnumerable<Button> FindButtons(Node root)
		{
			var stack = new Stack<Node>();
			stack.Push(root);
			while (stack.Count > 0)
			{
				var n = stack.Pop();
				if (n is Button b)
					yield return b;
				foreach (var child in n.GetChildren())
					stack.Push(child);
			}
		}
	}
}
