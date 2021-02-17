using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LAViD.LibrasKe.Avatar;

namespace LAViD.LibrasKe.Playback {

	public class Player : MonoBehaviour {

		public string recordName;

		private PlaybackBody body;
		private FrameData[] frames;

		private float frameSpeed = 1f / 3;
		private float frameCount = 0;

		private string filename = null;
		private volatile bool loaded = false;
		private bool playing = false;

		public delegate void PlayingStatusChangeFunction(bool playing);
		public event PlayingStatusChangeFunction PlayingStatusChangeEvent;

		public delegate void MusicLoadedFunction();
		public event MusicLoadedFunction MusicLoadedEvent;

		#region Life cycle

		void Start()
		{
			body = new PlaybackBody(gameObject.GetComponent<AvatarBody>());
		}

		void Update()
		{
			if (playing)
			{
				FrameData frame = frames[(int) frameCount];

				for (TrackedType type = TrackedType.First; type <= TrackedType.Last; type++)
				{
					body[type].Joint.rotation = frame[type].quaternion;
				}

				frameCount += frameSpeed;

				if ((int) frameCount >= frames.Length)
				{
					Debug.Log("Playing finished.");

					frameCount = 0;
					playing = false;
				}
			}
		}

		#endregion Life cycle

		public bool Loaded
		{
			get { return loaded; }
		}

		public bool Playing
		{
			get { return playing; }
		}

		public void Load()
		{
			if (loaded) throw new InvalidOperationException("Record " + recordName + " is already loaded.");

			filename = System.IO.Path.Combine(Application.dataPath, "Libraske/Resources/Recordings/" + recordName + ".lkd");
			Debug.Log("Loading " + filename + ".");

			if (System.IO.File.Exists(filename))
			{
				new Thread(new ThreadStart(loadAsync)).Start();
				StartCoroutine(WaitForLoadingComplete());
			}
			else
				Debug.Log("There is not file named " + filename + ".");
		}

		private void loadAsync()
		{
			using (System.IO.FileStream fs = System.IO.File.Open(filename, System.IO.FileMode.Open))
			{
				long size = fs.Length / ((int) TrackedType.Count * JointData.BytesCount);

				Debug.Log("Size: " + fs.Length);
				Debug.Log("Espected number os frames: " + size);

				frames = new FrameData[size - 1];
				byte[] data = new byte[JointData.BytesCount];
				float[] joint = new float[4];

				int i = 0;
                for (; fs.CanRead && i < size - 1; i++)
				{
					//Debug.Log("Reading a frame.");

					FrameData frame = new FrameData();

					for (TrackedType type = TrackedType.First; fs.CanRead && type <= TrackedType.Last; type++)
					{
						fs.Read(data, 0, JointData.BytesCount);
						Buffer.BlockCopy(data, 0, joint, 0, JointData.BytesCount);

						frame[type] = new JointData(joint);
					}

					frames[i] = frame;
					//Debug.Log("[" + i + "]: " + frames[i]);
				}

				Debug.Log("Read " + i + " frames.");
			}

			Debug.Log("Loading done!");

			loaded = true;
        }

		public IEnumerator WaitForLoadingComplete()
		{
			while (!loaded) yield return new WaitForFixedUpdate();

			if (MusicLoadedEvent != null)
				MusicLoadedEvent();
		}

		public void Play()
		{
			if (!loaded) throw new InvalidOperationException("Record " + recordName + " wasn't loaded.");

			playing = true;
			PlayingStatusChangeEvent(playing);
		}

	}

}