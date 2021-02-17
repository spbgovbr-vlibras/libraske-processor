using Microsoft.Kinect.Face;
using UnityEngine;
using Windows.Kinect;

namespace LAViD.LibrasKe {

	public enum BodySide {
		Left = 0,
		Right = 1
	}

	public enum Finger {
		Thumb = 0,
		Index = 1,
		Middle = 2,
		Ring = 3,
		Pinky = 4,

		First = 0,
		Last = 4,
		Count = 5
	}

	public static class Definitions {

		public static Vector3 FromSingleValue(this Vector3 vector, float value)
		{
			return new Vector3(value, value, value);
		} 

		public static Vector3 ToVector(this CameraSpacePoint point)
		{
			return new Vector3(point.X, point.Y, point.Z);
		}

		public static Vector2 ToVector2(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static Vector3 ToVector3(this Vector2 vector)
		{
			return new Vector3(vector.x, vector.y, 0);
		}

		public static Quaternion ToQuaternion(this Vector3 vector)
		{
			return Quaternion.Euler(vector.x, vector.y, vector.z);
		}

		public static Quaternion Euler(this Quaternion quaternion, Vector3 eulerAngles)
		{
			return Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
		}

		public static Vector2 ToVector(this Point point)
		{
			return new Vector2(point.X, point.Y);
		}

		public static Vector3 WithX(this Vector3 vector, float x)
		{
			return new Vector3(x, vector.y, vector.z);
		}

		public static Vector3 WithY(this Vector3 vector, float y)
		{
			return new Vector3(vector.x, y, vector.z);
		}

		public static Vector3 WithZ(this Vector3 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}

		public static int Int(this BodySide side)
		{
			return (int) side;
		}

	}

}
