using Godot;
using System;
using System.Collections.Generic;

namespace LunyScratch
{
	/// <summary>
	/// Godot implementation of engine-agnostic UI surface. Expects a Control root node.
	/// Provides basic show/hide and variable binding by child name.
	/// </summary>
	public abstract partial class ScratchUI : Control, IEngineUI
	{
		private readonly Dictionary<String, UIVariableBinding> _bindings = new();

		protected Control Root => this;

		public void BindVariable(Variable variable)
		{
			if (variable == null)
			{
				GD.PrintErr("Variable is null in BindVariable");
				return;
			}
			var name = variable.Name;
			if (String.IsNullOrEmpty(name))
			{
				GD.PrintErr("Variable has no Name for binding");
				return;
			}
			if (_bindings.ContainsKey(name))
			{
				GD.Print($"UI element '{name}' already bound");
				return;
			}
			var node = Root?.FindChild(name, true, false);
			if (node == null)
			{
				GD.Print($"UI element named '{name}' not found under {Root?.Name}");
				return;
			}
			var binding = new UIVariableBinding(node, variable);
			_bindings.Add(name, binding);
		}

		public override void _ExitTree()
		{
			foreach (var kv in _bindings)
				kv.Value.Dispose();
			_bindings.Clear();
		}
	}
}
