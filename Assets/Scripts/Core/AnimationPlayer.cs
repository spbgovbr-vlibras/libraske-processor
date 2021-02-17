using UnityEngine;
using System.Collections;
using System;

namespace LAViD.LibrasKe {

	public class AnimationPlayer : MonoBehaviour {

		public KinectManager kinect;
		public ScoreSystem scoreSystem;
        public Animation playbackAnimator;
		public Animation visualAnimator;
		public bool showVisualPlayback;
		public float delaySeconds;
        
		private bool playing = false;

		public bool Playing
		{
			get { return playing; }
		}

		public void Load(AnimationClip music)
		{
			Debug.Log("AP.Load: Clips loaded in avatar: " + playbackAnimator.GetClipCount());

			if (playbackAnimator.GetClipCount() > 0)
			{
				foreach (AnimationClip clip in playbackAnimator)
				{
					playbackAnimator.RemoveClip(clip);
				}
			}

			playbackAnimator.AddClip(playbackAnimator.clip = music, music.name);

			if (showVisualPlayback)
				visualAnimator.AddClip(visualAnimator.clip = music, music.name);

			Debug.Log("AP.Load: Loaded");
		}

		public void Play()
		{
			if (!playing)
			{
				StartCoroutine(PlayInSequence());
				Debug.Log("AP.Play: Started PlayInSequence Coroutine");
			}
			else Debug.Log("AP.Play: Already playing");
		}

		public void Stop()
		{
			playbackAnimator.Stop();
			playbackAnimator.clip = null;

			if (showVisualPlayback)
			{
				visualAnimator.Stop();
				visualAnimator.clip = null;
			}

			playing = false;

			Debug.Log("AP.Stop: Stoped");
		}

		private IEnumerator PlayInSequence()
		{
			string musicName = playbackAnimator.clip.name;

			Debug.Log("Starting to play: " + musicName);

			if (showVisualPlayback)
				visualAnimator.Play(PlayMode.StopAll);
			
			yield return new WaitForSeconds(delaySeconds);
			playbackAnimator.Play(PlayMode.StopAll);

			playing = true;

			Debug.Log("AP.PlayInSequence: Started animations");
		}


	}

}