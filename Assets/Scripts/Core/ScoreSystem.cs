using UnityEngine;
using LAViD.LibrasKe.Playback;
using UnityEngine.UI;
using LAViD.LibrasKe.Avatar;
using Windows.Kinect;
using System.Collections.Generic;
using System.Collections;

namespace LAViD.LibrasKe {

	public class ScoreSystem : MonoBehaviour {

		public KinectManager kinect;
		public AvatarBody avatarBody;
		public AvatarBody playbackBody;
		public AnimationPlayer player;

		public float minimumScoreForFull = 0.7f;

		private PlaybackBody avatar;
		private PlaybackBody playback;

		private float playerScore = 0;
		private float totalScore = 0;
		private float scoreHistoryCache = 0;
		private float[] scoreHistory = new float[SecondsTracked];
		private float[] scoreRatioHistory = new float[SecondsTracked];
		private bool[] hasFullScoreHistory = new bool[SecondsTracked];
		private int historyFirst = 0;

		#region Collision system

		public float elbowRadius = 0.5f;
		public float wristRadius = 0.5f;

		public bool showAvatarColliders = false;
		public bool showPlaybackColliders = false;
		private bool showColliders = false;

		public Material sphereMaterial;
		public float sphereColorAlpha;

		private GameObject[] avatarSpheres = new GameObject[(int) TrackedType.Count];
		private GameObject[] playbackSpheres = new GameObject[(int) TrackedType.Count];

		private Color noCollisionColor;
		private Color avatarCollisionColor;
		private Color playbackCollisionColor;

		private Dictionary<TrackedType, float> collidersRadius;
		private Dictionary<TrackedType, Renderer> avatarCollidersRenderers;
		private Dictionary<TrackedType, Renderer> playbackCollidersRenderers;

		#endregion Collision system

		public static int SecondsTracked { get { return 5; } }

		public bool HasFullScoreOnLast(int seconds)
		{
			if (!player.Playing) return false; // || !kinect.IsInsideLimits
			return ScoreRatioOnLast(seconds) > minimumScoreForFull;
		}

		public float ScoreRatioOnLast(int seconds)
		{
			if (!player.Playing) return 0;
			return scoreRatioHistory[SecondsTracked - seconds];
		}

		private int indexOf(int second)
		{
			return (second + historyFirst) % SecondsTracked;
		}

		#region Unity life cycle

		void Start()
		{
			noCollisionColor = new Color(Color.white.r, Color.white.g, Color.white.b, sphereColorAlpha);
			avatarCollisionColor = new Color(Color.red.r, Color.red.g, Color.red.b, sphereColorAlpha);
			playbackCollisionColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, sphereColorAlpha);

			collidersRadius = new Dictionary<TrackedType, float>()
			{
				{ TrackedType.ShoulderLeft, elbowRadius },
				{ TrackedType.ElbowLeft, wristRadius },
				{ TrackedType.ShoulderRight, elbowRadius },
				{ TrackedType.ElbowRight, wristRadius }
			};

			avatar = new PlaybackBody(avatarBody);
			playback = new PlaybackBody(playbackBody);

			// Create colliders spheres
			if (showColliders = showAvatarColliders || showPlaybackColliders)
			{
				if (showAvatarColliders)
					avatarCollidersRenderers = new Dictionary<TrackedType, Renderer>();

				if (showPlaybackColliders)
					playbackCollidersRenderers = new Dictionary<TrackedType, Renderer>();

				for (TrackedType type = TrackedType.First; type <= TrackedType.Last; type++)
				{
					float radius = collidersRadius[type];

					if (showAvatarColliders)
					{
						Vector3 avatarJointPosition = avatar.Collisor[type].transform.position;
						avatarSpheres[(int) type] = createSphere(avatarJointPosition, radius);

						avatarCollidersRenderers.Add(type, avatarSpheres[(int) type].GetComponent<Renderer>());
					}

					if (showPlaybackColliders)
					{
						Vector3 playbackJointPosition = playback.Collisor[type].transform.position;
						playbackSpheres[(int) type] = createSphere(playbackJointPosition, radius);

						playbackCollidersRenderers.Add(type, playbackSpheres[(int) type].GetComponent<Renderer>());
					}
				}
			}

			// Score history
			for (int i = 0; i < SecondsTracked; i++)
			{
				scoreHistory[indexOf(i)] = 0;
				scoreRatioHistory[indexOf(i)] = 0;
				hasFullScoreHistory[indexOf(i)] = false;
			}

			StartCoroutine(ScoreChecker());
		}

		void Update()
		{
			if (!player.Playing) return;

			int frameScore = 0;

			for (TrackedType type = TrackedType.First; type <= TrackedType.Last; type++)
			{
				// Checks if avatar joint collides with playback joint
				bool collided = doesItCollide(
						avatar.Collisor[type].transform.position,
						playback.Collisor[type].transform.position,
						collidersRadius[type]
					);

				if (collided) frameScore++;
				if (showColliders) updateSphere(type, collided);
			}

			int totalFrameScore = (int) TrackedType.Count;

			scoreHistoryCache += frameScore;

			playerScore += frameScore;
			totalScore += totalFrameScore;
			GameData.score = playerScore / totalScore;
		}

		#endregion Unity life cycle

		private GameObject createSphere(Vector3 position, float radius)
		{
			GameObject sphere;

			sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = position;
			sphere.transform.localScale = new Vector3(radius, radius, radius);
			sphere.GetComponent<Renderer>().material = sphereMaterial;

			return sphere;
		}

		private void updateSphere(TrackedType type, bool collided)
		{
			if (collided)
			{
				if (showAvatarColliders)
					avatarCollidersRenderers[type].material.color = avatarCollisionColor;

				if (showPlaybackColliders)
					playbackCollidersRenderers[type].material.color = playbackCollisionColor;
			}
			else
			{
				if (showAvatarColliders)
					avatarCollidersRenderers[type].material.color = noCollisionColor;

				if (showPlaybackColliders)
					playbackCollidersRenderers[type].material.color = noCollisionColor;
			}

			if (showAvatarColliders)
				avatarSpheres[(int) type].transform.position = avatar.Collisor[type].transform.position;

			if (showPlaybackColliders)
				playbackSpheres[(int) type].transform.position = playback.Collisor[type].transform.position;
		}

		private bool doesItCollide(Vector3 jointPosition, Vector3 playbackJointPosition, float radius)
		{
			Vector3 delta = playbackJointPosition - jointPosition;
			return Vector3.Dot(delta, delta) < (radius * radius);
		}

		private IEnumerator ScoreChecker()
		{
			while (true)
			{
				do {
					yield return new WaitForSeconds(1);
				} while (!player.Playing);

				// Update external score
				GameData.score = playerScore / totalScore;

				// Clear the last second
				scoreHistory[indexOf(0)] = 0;
				scoreRatioHistory[indexOf(0)] = 0;
				hasFullScoreHistory[indexOf(0)] = false;

				// Calculates the score ratio on the last second
				float scoreRatioOnCache = scoreHistoryCache / (30 * 4);

				// Increments pointer to the first element
				historyFirst += (historyFirst + 1) % SecondsTracked;

				// Adds the score ratio to all seconds
				for (int i = 0; i < SecondsTracked; i++)
				{
					int index = indexOf(i);

					scoreHistory[index] += scoreHistoryCache;
					scoreRatioHistory[index] = (
							// Multiplies by the number of seconds counted
							scoreRatioHistory[index] * (SecondsTracked - i - 1) +
							// Adds the last second score
							scoreRatioOnCache
						// Divides by the number of seconds counted plus one (the last second)
						// Makes the sum with the last second proportional to the seconds
						// already counted
						) / (SecondsTracked - i);
					hasFullScoreHistory[index] = scoreRatioHistory[index] >= minimumScoreForFull;
				}

				// Clear the cache
				scoreHistoryCache = 0;
			}
		}

	}

}

/*

[ 5, 4, 3, 2, 1 ]
-> 0

[ 0, 4, 3, 2, 1 ]
[ 1, 5, 4, 3, 2 ]
-> 1

[ 1, 0, 4, 3, 2 ]
[ 2, 1, 5, 4, 3 ]
-> 2

*/
