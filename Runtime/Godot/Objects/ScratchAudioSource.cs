using Godot;

namespace LunyScratch
{
	internal sealed class ScratchAudioSource : IEngineAudioSource
	{
		private readonly GodotObject _audio;

		public ScratchAudioSource(GodotObject audio) => _audio = audio;

		public void Play()
		{
			switch (_audio)
			{
				case AudioStreamPlayer p:
					p.Play();
					break;
				case AudioStreamPlayer2D p2d:
					p2d.Play();
					break;
				case AudioStreamPlayer3D p3d:
					p3d.Play();
					break;
			}
		}
	}
}
