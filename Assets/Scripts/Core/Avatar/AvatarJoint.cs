using UnityEngine;
using Windows.Kinect;

namespace LAViD.LibrasKe.Avatar {

	public class AvatarJoint : MonoBehaviour {

		private Transform joint;

		private Vector3 initialPosition;
		private Vector3 initialAngles;
		private Quaternion initialOrientation;

		private Vector3 initialLocalPosition;
		private Vector3 initialLocalAngles;
		private Quaternion initialLocalOrientation;

		public Transform Joint {
			get { return joint; }
		}

		// Initial
		public Vector3 InitialPosition { get { return initialPosition; } }
		public Vector3 InitialRotation { get { return initialAngles; } }
		public Quaternion InitialOrientation { get { return initialOrientation; } }

		// Initial local
		public Vector3 InitialLocalPosition { get { return initialLocalPosition; } }
		public Vector3 InitialLocalRotation { get { return initialLocalAngles; } }
		public Quaternion InitialLocalOrientation { get { return initialLocalOrientation; } }

		// Access
		public Vector3 Position { get { return joint.transform.position; } }
		public Vector3 Rotation { get { return joint.transform.eulerAngles; } }
		public Quaternion Orientation { get { return joint.transform.rotation; } }

		// Access local
		public Vector3 LocalPosition { get { return joint.transform.localPosition; } }
		public Vector3 LocalRotation { get { return joint.transform.localEulerAngles; } }
		public Quaternion LocalOrientation { get { return joint.transform.localRotation; } }

		void Start()
		{
			joint = base.gameObject.transform;

			initialPosition = joint.position;
			initialAngles = joint.eulerAngles;
			initialOrientation = joint.rotation;

			initialLocalPosition = joint.localPosition;
			initialLocalAngles = joint.localEulerAngles;
			initialLocalOrientation = joint.localRotation;
		}
		
	}

}