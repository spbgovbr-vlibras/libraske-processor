using UnityEngine;

namespace LAViD.LibrasKe {

	public class AnimationStarter : MonoBehaviour {

		public AnimationPlayer player;
		public AnimationClip gonzagas;
		public AnimationClip pereira;
		public AnimationClip brito;

		void Start()
		{
			if (GameData.music == null)
			{
				Debug.Log("AS.S: No music selected.");
				return;
			}

			switch (GameData.music)
			{
				case GameData.Music_OsGonzagas_AmorDaGota:
					player.Load(gonzagas); player.Play();
					break;
				case GameData.Music_SeuPereira_CarimboDaPenha:
					player.Load(pereira); player.Play();
					break;
				case GameData.Music_BetoBrito_XoteMeiDeFeira:
					player.Load(brito); player.Play();
					break;
			}
		}

	}

}