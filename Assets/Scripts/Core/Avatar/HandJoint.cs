using System;
using UnityEngine;
using Windows.Kinect;

namespace LAViD.LibrasKe.Avatar {

	public class HandJoint : MonoBehaviour {

		public Transform[] thumb;
		public Transform[] index;
		public Transform[] middle;
		public Transform[] ring;
		public Transform[] pinky;

		public BodySide side;

		private JointType tipType;
		private JointType handType;
		private JointType wristType;

		private HandSettings openSettings;

		public Transform[] this[Finger finger] {
			get
			{
				if (finger == Finger.Thumb) return thumb;
				if (finger == Finger.Index) return index;
				if (finger == Finger.Middle) return middle;
				if (finger == Finger.Ring) return ring;
				if (finger == Finger.Pinky) return pinky;

				throw new ArgumentException("Invalid finger " + finger + ".");
			}
		}

		public JointType TipType {
			get { return tipType; }
		}

		public JointType HandType {
			get { return handType; }
		}

		public JointType WristType {
			get { return wristType; }
		}

		public HandSettings OpenSettings {
			get { return openSettings; }
		}

		void Start()
		{
			if (side == BodySide.Left)
			{
				tipType = JointType.HandTipLeft;
				handType = JointType.HandLeft;
				wristType = JointType.WristLeft;
			}
			else
			{
				tipType = JointType.HandTipRight;
				handType = JointType.HandRight;
				wristType = JointType.WristRight;
			}

			openSettings = new HandSettings(this);
		}

	}

}