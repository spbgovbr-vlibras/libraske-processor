using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

namespace LAViD.LibrasKe.Avatar {

	public class AvatarBody : MonoBehaviour {

		public AvatarJoint root;

		// Head
		public AvatarJoint head;
		public AvatarJoint neck;

		// Spine
		public AvatarJoint spineShoulder;
		public AvatarJoint spineMid;
		public AvatarJoint spineBase;

		// Left arm
		public AvatarJoint shoulderLeft;
		public AvatarJoint elbowLeft;
		public AvatarJoint wristLeft;

		// Right arm
		public AvatarJoint shoulderRight;
		public AvatarJoint elbowRight;
		public AvatarJoint wristRight;

		// Left leg
		public AvatarJoint hipLeft;

		// Right leg
		public AvatarJoint hipRight;

		private static Dictionary<JointType, JointType> rotationHolders = new Dictionary<JointType, JointType>()
		{
			{ JointType.Neck,			JointType.Head },

			{ JointType.SpineShoulder,	JointType.Neck },
			{ JointType.SpineMid,		JointType.SpineShoulder },
			{ JointType.SpineBase,		JointType.SpineMid },

			{ JointType.ShoulderLeft,	JointType.ElbowLeft },
			{ JointType.ElbowLeft,		JointType.WristLeft },
			{ JointType.WristLeft,		JointType.HandLeft },

			{ JointType.ShoulderRight,   JointType.ElbowRight },
			{ JointType.ElbowRight,      JointType.WristRight },
			{ JointType.WristRight,      JointType.HandRight }
		};

		public static Dictionary<JointType, JointType> RotationHolders
		{
			get { return rotationHolders; }
		}

		public AvatarJoint this[JointType type]
		{
			get
			{
				switch (type)
				{
					case JointType.Head: return head;
					case JointType.Neck: return neck;

					case JointType.SpineShoulder: return spineShoulder;
					case JointType.SpineMid: return spineMid;
					case JointType.SpineBase: return spineBase;

					case JointType.ShoulderLeft: return shoulderLeft;
					case JointType.ElbowLeft: return elbowLeft;
					case JointType.WristLeft: return wristLeft;

					case JointType.ShoulderRight: return shoulderRight;
					case JointType.ElbowRight: return elbowRight;
					case JointType.WristRight: return wristRight;

					case JointType.HipLeft: return hipLeft;
					case JointType.HipRight: return hipRight;

					default: return null;
				}
			}
		}

		public static Quaternion GetJointOrientation(Body body, JointType type)
		{
			Windows.Kinect.Vector4 orientationVector = body.JointOrientations[type].Orientation;
			return new Quaternion(
					orientationVector.X, orientationVector.Y,
					orientationVector.Z, orientationVector.W
				);
		}


	}

}