using Godot;

namespace LunyScratch
{
	/// <summary>
	/// Godot implementation of the Scratch HUD. Acts as a simple marker that also inherits ScratchUI functionality.
	/// Expects to be attached to a Control node named "HUD" at the scene root.
	/// </summary>
	[GlobalClass]
	public sealed partial class ScratchHUD : ScratchUI, IEngineHUD
	{
	}
}
