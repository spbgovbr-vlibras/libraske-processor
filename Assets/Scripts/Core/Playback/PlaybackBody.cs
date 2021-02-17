using LAViD.LibrasKe.Avatar;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

namespace LAViD.LibrasKe.Playback {

	public class PlaybackBody {

		private readonly AvatarJoint shoulderLeft;
		private readonly AvatarJoint elbowLeft;
		private readonly AvatarJoint wristLeft;
		private readonly AvatarJoint shoulderRight;
		private readonly AvatarJoint elbowRight;
		private readonly AvatarJoint wristRight;

		public readonly Dictionary<TrackedType, AvatarJoint> trackedJointCollisor;

		public PlaybackBody(AvatarBody body)
		{
			shoulderLeft = body.shoulderLeft;
			elbowLeft = body.elbowLeft;
			wristLeft = body.wristLeft;
			shoulderRight = body.shoulderRight;
			elbowRight = body.elbowRight;
			wristRight = body.wristRight;

			trackedJointCollisor = new Dictionary<TrackedType, AvatarJoint>()
			{
				{ TrackedType.ShoulderLeft, elbowLeft },
				{ TrackedType.ElbowLeft, wristLeft },
				{ TrackedType.ShoulderRight, elbowRight },
				{ TrackedType.ElbowRight, wristRight }
			};
		}

		public AvatarJoint this[TrackedType type]
		{
			get {
				switch (type)
				{
					case TrackedType.ShoulderLeft: return shoulderLeft;
					case TrackedType.ElbowLeft: return elbowLeft;
					case TrackedType.ShoulderRight: return shoulderRight;
					case TrackedType.ElbowRight: return elbowRight;
				}

				throw new System.ArgumentException("Argument must be from Playback.Type.");
			}
		}

		public Dictionary<TrackedType, AvatarJoint> Collisor
		{
			get { return trackedJointCollisor; }
		}

	}

}