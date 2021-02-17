using UnityEngine;
using Windows.Kinect;
using System;
using UnityEngine.UI;

namespace LAViD.LibrasKe.Avatar {

	public class HandsController : MonoBehaviour {

		private const int HighConfidenceFramesNumber = 7;

		public KinectManager manager;

		public bool basic = true;
		public bool follow = false;

		public HandJoint left;
		public HandJoint right;

		public ScoreSystem scoreSystem;
		public HandJoint followLeftHand;
		public HandJoint followRightHand;

		private JointType handType;
		private JointType tipType;
		private HandSettings settings;

		private int handStateFrameCount = 0;
		private float[] minRatio = new float[] { 100f, 100f };
		private float[] maxRatio = new float[] { -100f, -100f };

		private bool[] handClosed = new bool[] { false, false };

		void Update()
		{
			Body body = manager.Body;

			if (body != null)
			{
				if (follow && scoreSystem.HasFullScoreOnLast(5))
				{
					followHand(left, followLeftHand);
					followHand(right, followRightHand);
				}
				else if (basic)
				{
					checkHand(body, BodySide.Left);
					checkHand(body, BodySide.Right);
				}
				else
				{
					deepHandDetection(body, BodySide.Left);
					deepHandDetection(body, BodySide.Right);
				}
			}
		}
		
		public bool IsHandClosed(BodySide side)
		{
			return handClosed[side.Int()];
		}

		private void deepHandDetection(Body body, BodySide side)
		{
			HandJoint hand = side == BodySide.Left ? left : right;
			
			if ((side == BodySide.Right && body.HandRightState == HandState.Closed)
					|| (side == BodySide.Left && body.HandLeftState == HandState.Closed))
			{
				bruteChange(hand, HandSettings.Closed[hand.side.Int()]);
				return;
			}

			Vector3 tipPos = body.Joints[hand.TipType].Position.ToVector();
			Vector3 handPos = body.Joints[hand.HandType].Position.ToVector();
			Vector3 wristPos = body.Joints[hand.WristType].Position.ToVector();

			float top = Vector2.Distance(tipPos.ToVector2(), handPos.ToVector2());
			float bottom = Vector2.Distance(handPos.ToVector2(), wristPos.ToVector2());
			float ratio = top / bottom;

			if (ratio < minRatio[hand.side.Int()]) minRatio[hand.side.Int()] = ratio;
			if (ratio > maxRatio[hand.side.Int()]) maxRatio[hand.side.Int()] = ratio;

			minRatio[hand.side.Int()] = minRatio[hand.side.Int()] < 0.5f ? 0.7f : minRatio[hand.side.Int()];
			maxRatio[hand.side.Int()] = maxRatio[hand.side.Int()] > 1.6f ? 1.6f : maxRatio[hand.side.Int()];

			processHand(hand, minRatio[hand.side.Int()], maxRatio[hand.side.Int()], ratio);

			if (GameObject.Find("TextAxis") != null)
			{
				Text axisText = GameObject.Find("TextAxis").GetComponent<Text>();
				axisText.text = "ratio: " + ratio + "\n";
				axisText.text += "min: " + minRatio + "\n";
				axisText.text += "max: " + maxRatio + "\n";
            }
		}

		private void checkHand(Body body, BodySide side)
		{
			HandJoint hand = side == BodySide.Left ? left : right;
			HandState state = side == BodySide.Left ? body.HandLeftState : body.HandRightState;
			TrackingConfidence confidence = side == BodySide.Left ? body.HandLeftConfidence : body.HandRightConfidence;

			if (state == HandState.Closed)
			{
				if (confidence == TrackingConfidence.High || handStateFrameCount == 0)
				{
					bruteChange(hand, HandSettings.Closed[(int) side]);
					handStateFrameCount = HighConfidenceFramesNumber;
					handClosed[side.Int()] = true;
				}
            }

			/*else if (state == HandState.Lasso)
				bruteChange(hand, HandSettings.Lasso[(int) side]);*/

			else if (state == HandState.Open)
			{
				if (confidence == TrackingConfidence.High || handStateFrameCount == 0)
				{
					bruteChange(hand, hand.OpenSettings);
					handStateFrameCount = HighConfidenceFramesNumber;
					handClosed[side.Int()] = false;
				}
			}

			else if (handStateFrameCount == 0)
			{
				bruteChange(hand, hand.OpenSettings, 0.1f);
				handClosed[side.Int()] = false;
			}

			if (handStateFrameCount > 0) handStateFrameCount--;
		}
		
		private static void bruteChange(HandJoint hand, HandSettings settings)
		{
			bruteChange(hand, settings, 0.5f);
		}

		/* Process each finger. */
		private static void bruteChange(HandJoint hand, HandSettings settings, float smoothFactor)
		{
			for (Finger finger = Finger.Thumb; finger <= Finger.Pinky; finger++)
				processFinger(hand[finger], settings[finger], null, smoothFactor);
		}

		private static void processFinger(Transform[] node, Vector3[] angles, Vector3[] defaultSettings)
		{
			processFinger(node, angles, defaultSettings, 0.5f);
		}

		private static void processFinger(Transform[] node, Vector3[] angles, Vector3[] defaultSettings, float smoothFactor)
		{
			for (int i = 0; i < node.Length; i++)
			{
				Vector3 jointDefaultSettings;

				if (defaultSettings == null)
					jointDefaultSettings = node[i].localEulerAngles;
				else
					jointDefaultSettings = defaultSettings[i];

				//node[i].localEulerAngles = getFingerMovement(angles[i], jointDefaultSettings);
				Quaternion rotation = Quaternion.Euler(getFingerMovement(angles[i], jointDefaultSettings));
                node[i].localRotation = Quaternion.Slerp(node[i].localRotation, rotation, 0.5f);
			}
		}

		private static Vector3 getFingerMovement(Vector3 angles, Vector3 defaultSettings)
		{
			return new Vector3(
					angles.x == HandSettings.None ? defaultSettings.x : angles.x,
					angles.y == HandSettings.None ? defaultSettings.y : angles.y,
					angles.z == HandSettings.None ? defaultSettings.z : angles.z
				);
		}

		private static void processHand(HandJoint hand, float minRatio, float maxRatio, float ratio)
		{
			//Debug.Log("ratio: " + minRatio + " >= " + ratio + " <= " + maxRatio);

			ratio -= minRatio - 0.4f;
			maxRatio -= minRatio - 0.4f;
			minRatio = 0.4f;

			ratio /= maxRatio;
			maxRatio = 1f;

			if (ratio > maxRatio) ratio = maxRatio;
			if (ratio < minRatio) ratio = minRatio;

			//Debug.Log("~ratio: " + minRatio + " >= " + ratio + " <= " + maxRatio);

			HandSettings closedSettings = HandSettings.Closed[hand.side.Int()];

			for (Finger f = Finger.Index; f <= Finger.Pinky; f++)
			{
				Transform[] finger = hand[f];
				Vector3[] closed = closedSettings[f];
				Vector3[] open = hand.OpenSettings[f];

				for (int i = 0; i < finger.Length; i++)
				{
					Quaternion rotation = Quaternion.Slerp(
							closed[i].ToQuaternion(),
							open[i].ToQuaternion(),
							ratio
						);

					finger[i].localRotation = Quaternion.Slerp(
							finger[i].localRotation,
							rotation,
							0.3f
						);
				}
            }

			Transform[] thumb = hand[Finger.Thumb];

			for (int i = 0; i < thumb.Length; i++)
			{
				thumb[i].localRotation = Quaternion.Slerp(
						thumb[i].localRotation,
						hand.OpenSettings[Finger.Thumb][i].ToQuaternion(),
						0.5f
					);
			}
		}

		private void followHand(HandJoint hand, HandJoint followed)
		{
			for (Finger fingerType = Finger.First; fingerType <= Finger.Last; fingerType++)
			{
				Transform[] finger = hand[fingerType];
				Transform[] followedFinger = followed[fingerType];

				for (int i = 0; i < finger.Length; i++)
				{
					finger[i].localRotation = Quaternion.Slerp(
							finger[i].localRotation, followedFinger[i].localRotation, 0.7f
						);
				}
			}
		}

	}

}