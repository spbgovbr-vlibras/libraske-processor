using UnityEngine;

namespace LAViD.LibrasKe.Playback {

	public enum TrackedType {
		First = 0,
		
		ShoulderLeft = 0,
		ElbowLeft = 1,

		ShoulderRight = 2,
		ElbowRight = 3,

		Last = 3,
		Count = 4
	}

	public class JointData {

		private float[] data;

		public JointData(float[] values)
		{
			if (values.Length != 4)
				throw new System.ArgumentException("The array must have 4 elements.");

			data = (float[]) values.Clone();
		}

		public JointData(Quaternion quaternion)
		{
            data = new float[4];

			data[0] = quaternion.x;
			data[1] = quaternion.y;
			data[2] = quaternion.z;
			data[3] = quaternion.w;
		}

		public float[] Data
		{
			get { return data; }
		}

		public Quaternion quaternion
		{
			get { return new Quaternion(data[0], data[1], data[2], data[3]); }
		}

		public static int BytesCount
		{
			get { return 4 * sizeof(float); }
		}

		public override string ToString()
		{
			return "[ " + data[0] + ", " + data[1] + ", " + data[2] + ", " + data[3] + " ]";
		}

	}

	public class FrameData {

		private JointData[] data = new JointData[(int) TrackedType.Count];

		public JointData this[TrackedType type]
		{
			get { return data[(int) type]; }
			set { data[(int) type] = value; }
		}

		public override string ToString()
		{
			string str = "Frame {";

			for (TrackedType type = TrackedType.First; type <= TrackedType.Last; type++)
			{
				str += " " + this[type];
			}

			return str + " }";
		}

	}

}
