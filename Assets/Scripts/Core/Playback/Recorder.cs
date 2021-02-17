using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LAViD.LibrasKe.Avatar;
using UnityEngine.UI;

namespace LAViD.LibrasKe.Playback {

	public class Recorder : MonoBehaviour {

		public KinectManager manager;
		public string recordName = "libraske_" + DateTime.Now.ToShortTimeString();
		
		public RawImage indicator;

		private PlaybackBody body;
		private List<FrameData> frames = new List<FrameData>();

		private bool playing = false;

		void Start()
		{
			body = new PlaybackBody(gameObject.GetComponent<AvatarBody>());
		}

		void LateUpdate()
		{
			if (!playing) return;
			if (manager.Body == null && frames.Count > 0)
			{
				frames.Add(frames[frames.Count - 1]);
				return;
			}

			FrameData frame = new FrameData();

			for (TrackedType type = TrackedType.First; type <= TrackedType.Last; type++)
			{
				frame[type] = new JointData(body[type].Joint.rotation);
			}

			frames.Add(frame);
			Debug.Log(frames.Count + " frames captured.");
		}

		public void Play()
		{
			playing = true;
			indicator.color = Color.red;
		}

		public void Play(int delaySeconds)
		{
			StartCoroutine(StartCount(delaySeconds));
		}

		public void Stop()
		{
			playing = false;
			indicator.color = Color.white;

			if (frames.Count > 0)
			{
				save();
				frames.Clear();
				indicator.color = Color.blue;
			}
		}

		public void Cancel()
		{
			playing = false;
			indicator.color = Color.white;

			frames.Clear();
		}

		private IEnumerator StartCount(int delaySeconds)
		{
			for (int i = 0; i < delaySeconds; i++)
			{
				indicator.color = Color.red;
				yield return new WaitForSeconds(0.5f);

				indicator.color = Color.white;
				yield return new WaitForSeconds(0.5f);
			}

			Play();
		}

		private void save()
		{
			Debug.Log("Starting to save frames.");
			Debug.Log("Size espected: " + (frames.Count * (int) TrackedType.Count * JointData.BytesCount) + ".");

			string filename = System.IO.Path.Combine(Application.dataPath, recordName + ".lkd");

			using (System.IO.FileStream fs = System.IO.File.Create(filename))
			{
				for (int i = 0; i < frames.Count; i++)
				{
					for (TrackedType type = TrackedType.First; type <= TrackedType.Last; type++)
					{
						JointData joint = frames[i][type];
						byte[] data = new byte[JointData.BytesCount];

						Buffer.BlockCopy(joint.Data, 0, data, 0, JointData.BytesCount);
						fs.Write(data, 0, JointData.BytesCount);
                    }
				}

				Debug.Log("Size: " + fs.Length);
			}

			Debug.Log(frames.Count + " frames saved to " + filename + ".");
		}

	}

}