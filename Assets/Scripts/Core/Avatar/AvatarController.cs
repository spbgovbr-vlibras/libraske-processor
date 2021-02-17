using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

namespace LAViD.LibrasKe.Avatar {

	public class AvatarController : MonoBehaviour {
		
		public KinectManager manager;
		public ScoreSystem scoreSystem;
		public AvatarBody followedAvatar;

		public bool follow = false;

		private HandsController hands;
		private AvatarBody avatar;
		private Body body;

		private const float InferredSmoothFactor = 0.2f;
		private const float TrackedSmoothFactor = 0.3f;
		private const float DistanceForNeckOverlaping = 0.3f;
		private const float DistanceForElbowOverlaping = 1.5f;
		private const float ElbowOverlapSmoothFactorMultiplier = 0.3f;
		private const float ShoulderOverlapSmoothFactorMultiplier = 0.7f;

		private int outsideLimitsCounter = 0;

		void Start()
		{
			avatar = base.gameObject.GetComponent<AvatarBody>();
			hands = base.gameObject.GetComponent<HandsController>();
		}

		void Update()
		{
			if (!manager.HasTotalTracking) return;
			if (!manager.IsInsideLimits) return;

			body = manager.Body;
			if (body != null)
			{
				// Decreases elbow's and shoulder's smooth factor if wrist and elbow are too close
				bool isRightArmOverlaping = Vector2.Distance(
						body.Joints[JointType.ElbowRight].Position.ToVector().ToVector2(),
						body.Joints[JointType.WristRight].Position.ToVector().ToVector2()
					) <= DistanceForElbowOverlaping;
				bool isLeftArmOverlaping = Vector2.Distance(
						body.Joints[JointType.ElbowLeft].Position.ToVector().ToVector2(),
						body.Joints[JointType.WristLeft].Position.ToVector().ToVector2()
					) <= DistanceForElbowOverlaping;

				// Normalizers
				Quaternion normalizer = Quaternion.Euler(0, 0, -90f);
				Quaternion elbowNormalizer = Quaternion.Euler(0, 180f, -90f);

				rotateNeck(avatar.neck);

				// Spine
				rotateJoint(JointType.SpineShoulder, normalizer);
				rotateJoint(JointType.SpineMid, normalizer);
				rotateJoint(JointType.SpineBase, normalizer);

				// Left arm (mirrored right arm)
				rotateJoint(JointType.ShoulderLeft, normalizer, getHolderSmoothFactor(JointType.ShoulderLeft) *
						(isLeftArmOverlaping ? ShoulderOverlapSmoothFactorMultiplier : 1f)
					);
				rotateJoint(JointType.ElbowLeft, elbowNormalizer, getHolderSmoothFactor(JointType.ElbowLeft) *
						(isLeftArmOverlaping ? ElbowOverlapSmoothFactorMultiplier : 1f)
					);

				// Right arm (mirrored left arm)
				rotateJoint(JointType.ShoulderRight, normalizer, getHolderSmoothFactor(JointType.ShoulderRight) *
						(isRightArmOverlaping ? ShoulderOverlapSmoothFactorMultiplier : 1f)
					);
				rotateJoint(JointType.ElbowRight, elbowNormalizer, getHolderSmoothFactor(JointType.ElbowRight) *
						(isRightArmOverlaping ? ElbowOverlapSmoothFactorMultiplier : 1f)
					);
				
				if (follow && scoreSystem.HasFullScoreOnLast(5))
				{
					followPlayback(BodySide.Left);
					followPlayback(BodySide.Right);
				}
				else
				{
					if (!hands.IsHandClosed(BodySide.Left))
						rotateHand(JointType.WristLeft, Quaternion.Euler(0, 225f, -90f));

					if (!hands.IsHandClosed(BodySide.Right))
						rotateHand(JointType.WristRight, Quaternion.Euler(0, -225f, -90f));
				}

				move(body);
			}
		}

		private void rotateNeck(AvatarJoint neck)
		{
			// Positions of head and neck
			Vector3 headPosition = body.Joints[JointType.Head].Position.ToVector();
			Vector3 neckPosition = body.Joints[JointType.Neck].Position.ToVector();

			// Returns if has any hand too close to the head,
			// keeping the last head rotation and avoiding inaccuracy
			{
				// Positions of hands
				Vector3 leftHand = body.Joints[JointType.HandLeft].Position.ToVector();
				Vector3 rightHand = body.Joints[JointType.HandRight].Position.ToVector();

				// Distance between the head and each hand
				float leftHandDistance = Vector3.Distance(headPosition, leftHand);
				float rightHandDistance = Vector3.Distance(headPosition, rightHand);
				
				// Returns if too close
				if (leftHandDistance <= DistanceForNeckOverlaping || rightHandDistance <= DistanceForNeckOverlaping)
					return;
			}

			// Spine base orientation
			Quaternion baseOrientation = AvatarBody.GetJointOrientation(body, JointType.SpineBase);

			// Used to make rotation relative to player rotation
			Vector3 baseAngles = baseOrientation.eulerAngles;

			// Normalizes y angle
			baseAngles.y += 180f;

			// Vector from neck to head
			Vector3 vector = headPosition - neckPosition;

			// Multiply by 1.4f to make rotations 40% clearer
			float xAngle = -Mathf.Atan(vector.z / vector.y) * (180f / Mathf.PI) * 1.4f;
			float zAngle = Mathf.Atan(vector.x / vector.y) * (180f / Mathf.PI) * 1.4f;

			// Orientates avatar's head to camera
			float yAngle = 180f - AvatarBody.GetJointOrientation(body, JointType.Neck).eulerAngles.y;
			
			float xOffset = 22f - neckPosition.y * neckPosition.z * 6.6f;

			// Adds 22f to x angle for normalization
			Vector3 angles = new Vector3(xAngle + xOffset, yAngle, zAngle);

			// Rotates by angle from initial rotation and normalize with base orientation
			Vector3 rotationVector = neck.InitialRotation + baseAngles + angles;

			Quaternion rotation = Quaternion.Euler(rotationVector);
			neck.Joint.rotation = Quaternion.Lerp(neck.Joint.rotation, rotation, TrackedSmoothFactor);
		}

		private void rotateJoint(JointType type, Quaternion normalizer)
		{
			rotateJoint(type, normalizer, getHolderSmoothFactor(type));
		}

		private void rotateJoint(JointType type, Quaternion normalizer, float smoothFactor)
		{
			AvatarJoint joint = avatar[type];

			if (body.Joints[type].TrackingState != TrackingState.NotTracked)
			{
				Quaternion rotation = AvatarBody.GetJointOrientation(body, AvatarBody.RotationHolders[type]) * normalizer;

				// Keeps rotation on X when hand is closed

				/*if ((type == JointType.ElbowLeft && hands.IsHandClosed(BodySide.Left))
					|| (type == JointType.ElbowRight && hands.IsHandClosed(BodySide.Right)))
				{
					Debug.Log(type);

					Quaternion currentLocalRotation = joint.Joint.localRotation;
					Vector3 currentLocalAngles = joint.Joint.localEulerAngles;

					joint.Joint.rotation = rotation;
					joint.Joint.localEulerAngles = joint.Joint.localEulerAngles.WithY(currentLocalAngles.y);
				
					joint.Joint.localRotation = Quaternion.Lerp(
							currentLocalRotation,
							joint.Joint.localRotation,
							smoothFactor
						);
				}
				else*/
				{
					joint.Joint.rotation = Quaternion.Lerp(
							joint.Joint.rotation,
							rotation,
							smoothFactor
						);
				}
			}
		}

		private float getSmoothFactor(JointType type)
		{
			if (body.Joints[type].TrackingState == TrackingState.Tracked)
				return TrackedSmoothFactor;
			else
				return InferredSmoothFactor;
		}

		private float getHolderSmoothFactor(JointType type)
		{
			return getSmoothFactor(AvatarBody.RotationHolders[type]);
		}

		private void rotateHand(JointType type, Quaternion normalizer)
		{
			JointType rotationHolder = AvatarBody.RotationHolders[type];

			if (body.Joints[rotationHolder].TrackingState != TrackingState.NotTracked)
			{
				AvatarJoint hand = avatar[type];

				Quaternion orientation = AvatarBody.GetJointOrientation(body, rotationHolder);
				Quaternion rotation = orientation * normalizer;
				Quaternion currentRotation = hand.Joint.rotation;
				Quaternion currentLocalRotation = hand.Joint.localRotation;

				hand.Joint.rotation = rotation;

				Vector3 angles = hand.Joint.localEulerAngles;
				
				if (angles.x > 180f) angles.x = angles.x - 360f;
				if (angles.y > 180f) angles.y = angles.y - 360f;
				if (angles.z > 180f) angles.z = angles.z - 360f;

				bool outsideLimits = true;
				float clamped;

				clamped = Mathf.Clamp(angles.x, -25f, 25f);
				if (clamped != angles.x) angles.x = clamped;
				else outsideLimits = false;

				clamped = Mathf.Clamp(angles.y, -43f, 40f);
				if (clamped != angles.y) angles.y = clamped;
				else outsideLimits = false;

				clamped = Mathf.Clamp(angles.z, -26f, 26f);
				if (clamped != angles.z) angles.z = clamped;
				else outsideLimits = false;
				
				if (outsideLimits) outsideLimitsCounter = -3;

				if (outsideLimitsCounter < 0)
				{
					outsideLimitsCounter++;

					if (outsideLimitsCounter == 0)
						outsideLimitsCounter = 30;

					hand.Joint.rotation = currentRotation;
				}
				else
				{
					float smoothFactor = getSmoothFactor(type);
					if (outsideLimitsCounter > 0)
					{
						smoothFactor *= 0.3f;
						outsideLimitsCounter--;
					}
					
					Quaternion anglesRotation = Quaternion.Euler(angles);
					hand.Joint.localRotation = Quaternion.Slerp(
							currentLocalRotation, anglesRotation, smoothFactor
						);
				}
			}
		}

		private void move(Body body)
		{
			Vector3 detected = body.Joints[JointType.SpineBase].Position.ToVector();
			Vector3 initial = avatar.root.InitialLocalPosition;
			Vector3 current = avatar.root.Joint.localPosition;
			Vector3 position = new Vector3(initial.x - detected.x, current.y, current.z);

			avatar.root.Joint.localPosition = Vector3.Lerp(current, position, InferredSmoothFactor);
		}

		private void followPlayback(BodySide side)
		{
			JointType shoulder = side == BodySide.Left ? JointType.ShoulderLeft : JointType.ShoulderRight;
			JointType elbow = side == BodySide.Left ? JointType.ElbowLeft : JointType.ElbowRight;
			JointType hand = side == BodySide.Left ? JointType.WristLeft : JointType.WristRight;
			
			float angle, ratio;

			// Shoulder

			angle = Quaternion.Angle(
					avatar[shoulder].Joint.localRotation,
					followedAvatar[shoulder].Joint.localRotation
				);
			ratio = (Mathf.Clamp(angle, 5f, 60f) - 5f) / 55f;

			avatar[shoulder].Joint.localRotation = Quaternion.Slerp(
					avatar[shoulder].Joint.localRotation,
					followedAvatar[shoulder].Joint.localRotation,
					ratio * 0.02f
				);

			// Elbow

			angle = Quaternion.Angle(
					avatar[elbow].Joint.localRotation,
					followedAvatar[elbow].Joint.localRotation
				);
			ratio = (Mathf.Clamp(angle, 5f, 60f) - 5f) / 55f;

			avatar[elbow].Joint.localRotation = Quaternion.Slerp(
					avatar[elbow].Joint.localRotation,
                    followedAvatar[elbow].Joint.localRotation,
					ratio * 0.05f
				);

			// Hand
			
			avatar[hand].Joint.localRotation = Quaternion.Slerp(
					avatar[hand].Joint.localRotation,
					followedAvatar[hand].Joint.localRotation,
					scoreSystem.HasFullScoreOnLast(1) ? 1f : 0.3f
				);
		}

	}

}