using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LAViD.LibrasKe.Avatar {

	public class HandSettings {

		public static readonly float None = 1.175494351e-38f;
		public static readonly Vector3 Keep = new Vector3(None, None, None);

		public static readonly HandSettings[] Closed = {
					HandSettings.CreateClosedSettings(BodySide.Left),
					HandSettings.CreateClosedSettings(BodySide.Right)
				};

		public static readonly HandSettings[] Lasso = {
					HandSettings.CreateLassoSettings(BodySide.Left),
					HandSettings.CreateLassoSettings(BodySide.Right)
				};

		public static readonly HandSettings[] OpenLimit = {
					HandSettings.CreateOpenLimitSettings(BodySide.Left),
					HandSettings.CreateOpenLimitSettings(BodySide.Right)
				};

		private Vector3[][] fingersAngles = new Vector3[5][];

		public Vector3[] this[Finger finger] {
			get
			{
				return fingersAngles[(int) finger];
			}
		}

		private HandSettings()
		{
			for (int i = 0; i < 5; i++)
				fingersAngles[i] = new Vector3[3];
		}

		public HandSettings(HandJoint hand) : this()
		{
			for (Finger finger = Finger.Thumb; finger <= Finger.Pinky; finger++)
			{
				for (int i = 0; i < 3; i++)
					this[finger][i] = hand[finger][i].localEulerAngles;
			}
		}

		private static HandSettings CreateClosedSettings(BodySide type)
		{
			HandSettings settings = new HandSettings();

			for (Finger finger = Finger.Index; finger <= Finger.Pinky; finger++)
			{
				settings[finger][0] = new Vector3(None, -90f, None);
				settings[finger][1] = new Vector3(None, -90f, None);
				settings[finger][2] = new Vector3(None, -45f, None);
			}

			if (type == BodySide.Left)
			{
				settings[Finger.Thumb][0] = new Vector3(None, -55f, -25f);
				settings[Finger.Thumb][1] = new Vector3(-50f, 10f, 55f);
				settings[Finger.Thumb][2] = new Vector3(None, None, 60f);
			}
			else
			{
				settings[Finger.Thumb][0] = new Vector3(None, -55f, 25f);
				settings[Finger.Thumb][1] = new Vector3(50f, -10f, -55f);
				settings[Finger.Thumb][2] = new Vector3(None, None, -60f);
			}

			return settings;
		}

		private static HandSettings CreateLassoSettings(BodySide type)
		{
			HandSettings settings = new HandSettings();

			for (Finger finger = Finger.Ring; finger <= Finger.Pinky; finger++)
			{
				settings[finger][0] = new Vector3(None, -90f, None);
				settings[finger][1] = new Vector3(None, -90f, None);
				settings[finger][2] = new Vector3(None, -45f, None);
			}

			if (type == BodySide.Left)
			{
				settings[Finger.Thumb][0] = new Vector3(None, -55f, -25f);
				settings[Finger.Thumb][1] = new Vector3(-50f, 10f, 55f);
				settings[Finger.Thumb][2] = new Vector3(None, None, 60f);
			}
			else
			{
				settings[Finger.Thumb][0] = new Vector3(None, -55f, 25f);
				settings[Finger.Thumb][1] = new Vector3(50f, -10f, -55f);
				settings[Finger.Thumb][2] = new Vector3(None, None, -60f);
			}

			return settings;
		}

		private static HandSettings CreateOpenLimitSettings(BodySide type)
		{
			HandSettings settings = new HandSettings();

			for (Finger finger = Finger.Index; finger <= Finger.Pinky; finger++)
			{
				settings[finger][0] = new Vector3(None, 30f, None);
				settings[finger][1] = new Vector3(None, 20f, None);
				settings[finger][2] = new Vector3(None, 5f, None);
			}

			settings[Finger.Thumb][0] = Keep;
			settings[Finger.Thumb][1] = new Vector3(None, None, -45f);
			settings[Finger.Thumb][2] = new Vector3(None, None, -30f);

			if (type == BodySide.Right)
			{
				settings[Finger.Thumb][1].z *= -1;
				settings[Finger.Thumb][2].z *= -1;
			}

			return settings;
		}

	}

}
