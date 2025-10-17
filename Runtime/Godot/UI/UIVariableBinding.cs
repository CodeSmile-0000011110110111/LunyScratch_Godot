using Godot;
using System;

namespace LunyScratch
{
	internal sealed class UIVariableBinding : IDisposable
	{
		private readonly Node _element;
		private readonly Variable _variable;

		public UIVariableBinding(Node element, Variable variable)
		{
			_element = element;
			_variable = variable;

			OnValueChanged(_variable); // set initial value
			_variable.OnValueChanged += OnValueChanged;
		}

		public void Dispose()
		{
			_variable.OnValueChanged -= OnValueChanged;
		}

		private void OnValueChanged(Variable variable)
		{
			if (_element is Label label)
			{
				if (variable.IsNumber)
					label.Text = variable.Number.ToString("N0");
				else
					label.Text = variable.String;
			}
			else
			{
				GD.PrintErr($"Unsupported UI element type: {_element?.GetType()} (name={_element?.Name})");
			}
		}
	}
}
