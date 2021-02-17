using UnityEngine;
using Microsoft.Kinect.Face;
using LAViD.LibrasKe;
using UnityEngine.UI;

namespace LAViD.LibrasKe.Avatar {

	public class FaceController : MonoBehaviour {

		private const float SmoothFactor = 0.4f;

		public KinectManager kinect;

		public AvatarJoint neck;

		public Transform mouth;
		public Transform leftBrow;
		public Transform rightBrow;
		public Transform jaw;

		private float closedMouthValue = -0.3276826f;
		private float openMouthValue = -0.39f;

		private Vector3 jawInitialPosition;
		private Vector3 jawInitialRotation;

		private float openJawPositionYVar;
		private float openJawRotationXVar;

		private float browDownVar;
		private float browUpVar;
		private float defaultBrow;

		void Start()
		{
			// Brows
			defaultBrow = leftBrow.localPosition.x;
			browDownVar = -defaultBrow - 1.15f;
			browUpVar = defaultBrow + 1.35f;

			// Jaw
			jawInitialPosition = jaw.localPosition;
			jawInitialRotation = jaw.localEulerAngles;
			openJawPositionYVar = -0.01f - jawInitialPosition.y;
			openJawRotationXVar = 26f - jawInitialRotation.x;
		}

		void Update()
		{
			var hdFace = kinect.HDFace;
			if (hdFace != null)
			{
				moveBrow(leftBrow, hdFace.AnimationUnits[FaceShapeAnimations.LefteyebrowLowerer]);
				moveBrow(rightBrow, hdFace.AnimationUnits[FaceShapeAnimations.RighteyebrowLowerer]);

				smile(hdFace.AnimationUnits[FaceShapeAnimations.LipPucker]);
				openMouth(hdFace.AnimationUnits[FaceShapeAnimations.JawOpen]);
				//rotateJaw(hdFace.AnimationUnits[FaceShapeAnimations.JawSlideRight]);
			}
		}

		private void smile(float coef)
		{
			coef = coef > 0.6f ? 0 : 1 - coef / 0.6f;

			float value = closedMouthValue + coef * (openMouthValue - closedMouthValue);
			mouth.localPosition = Vector3.Slerp(mouth.localPosition, mouth.localPosition.WithX(value), SmoothFactor);
		}

		private void openMouth(float coef)
		{
			float jawPositionY = jawInitialPosition.y + coef * openJawPositionYVar;
			jaw.localPosition = Vector3.Slerp(jaw.localPosition, jaw.localPosition.WithY(jawPositionY), SmoothFactor);

			float jawRotationX = jawInitialRotation.x + coef * openJawRotationXVar;
			jaw.localEulerAngles = Vector3.Slerp(jaw.localEulerAngles, jaw.localEulerAngles.WithX(jawRotationX), SmoothFactor);
		}

		private void rotateJaw(float coef)
		{
			Vector3 jawRotation = jaw.localEulerAngles.WithZ(jawInitialRotation.z + coef * 35f);
			jaw.localEulerAngles = Vector3.Slerp(jaw.localEulerAngles, jawRotation, SmoothFactor);
		}

		private void moveBrow(Transform brow, float value)
		{
			float browX = defaultBrow;

			if (value < 0)
				browX += value * browDownVar;

			else if (value > 0)
				browX += value * browUpVar;

			brow.localPosition = Vector3.Lerp(brow.localPosition, brow.localPosition.WithX(browX), SmoothFactor * 2 / 3);
		}

	}

}