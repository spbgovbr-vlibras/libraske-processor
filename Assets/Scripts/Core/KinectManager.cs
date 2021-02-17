using UnityEngine;
using Windows.Kinect;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Linq;
using System.Collections.Generic;

namespace LAViD.LibrasKe {

	public class KinectManager : MonoBehaviour {

		public float zLimit = 1.2f;

		public bool trackBody = true;
		public bool trackFace = true;
		public bool trackHDFace = true;

		private KinectSensor sensor = null;
		private BodyFrameReader bodyReader = null;
		private Body[] bodies;

		private FaceFrameSource faceSource;
		private FaceFrameReader faceReader;

		private HighDefinitionFaceFrameSource hdFaceSource;
		private HighDefinitionFaceFrameReader hdFaceReader;
		
		private bool hasTotalTracking = false;
		private bool isInsideLimits = false;

		private Body trackedBody;
		private FaceFrameResult trackedFace;
		private FaceAlignment trackedHDFace;

		private int displayWidth, displayHeight;

		private delegate void AcquireFunction();
		private event AcquireFunction Acquire;

		private delegate void DisposeFunction();
		private event DisposeFunction Dispose;

		// Specify the required face frame results
		FaceFrameFeatures faceFrameFeatures =
				  FaceFrameFeatures.BoundingBoxInColorSpace
				| FaceFrameFeatures.PointsInColorSpace;

		public Body[] getBodies() {
			return bodies;
		}

		public Body Body {
			get
			{
				if (trackedBody != null && trackedBody.IsTracked)
					return trackedBody;

				if (bodies != null)
					return trackedBody = bodies.Where(b => b != null && b.IsTracked).FirstOrDefault();

				return null;
			}
		}

		public FaceFrameResult Face
		{
			get { return trackedFace; }
		}

		public FaceAlignment HDFace
		{
			get { return trackedHDFace; }
		}

		public bool HasTotalTracking
		{
			get { return hasTotalTracking; }
		}

		public bool IsInsideLimits
		{
			get { return isInsideLimits; }
		}

		#region Life cycle

		void Start()
		{
			if (!trackBody && !trackFace && !trackHDFace) return;

            if (GameData.sensor == null)
            {
                GameData.sensor = KinectSensor.GetDefault();
            }

            sensor = GameData.sensor;

			if (sensor != null)
			{
				// Get the color frame details and set the display specifics
				FrameDescription frameDescription = sensor.ColorFrameSource.FrameDescription;
				displayWidth = frameDescription.Width;
				displayHeight = frameDescription.Height;

				// Body
				if (trackBody)
				{
                    if (GameData.reader == null)
    					GameData.reader = sensor.BodyFrameSource.OpenReader();

                    bodyReader = GameData.reader;

                    if (bodyReader != null)
					{
						bodies = new Body[sensor.BodyFrameSource.BodyCount];
						Acquire += acquireBody;
						Dispose += bodyReader.Dispose;
					}
				}

				// Face
				if (trackFace)
				{
					faceSource = FaceFrameSource.Create(sensor, 0, faceFrameFeatures);
					faceReader = faceSource.OpenReader();

					if (faceReader != null)
					{
						Acquire += acquireFace;
						Dispose += faceReader.Dispose;
					}
				}

				// HD Face
				if (trackHDFace)
				{
					hdFaceSource = HighDefinitionFaceFrameSource.Create(sensor);
					hdFaceReader = hdFaceSource.OpenReader();

					if (hdFaceReader != null)
					{
						trackedHDFace = FaceAlignment.Create();
						Acquire += acquireHDFace;
						Dispose += hdFaceReader.Dispose;
					}
				}

				if (!sensor.IsOpen) sensor.Open();
			}
		}

		void Update()
		{
			if (Acquire != null) Acquire();
		}

		void OnAplicationQuit()
		{
			if (Dispose != null)
			{
				Dispose();

				bodyReader = null;
				faceReader = null;
				hdFaceReader = null;
			}
			
			if (sensor != null)
			{
				if (sensor.IsOpen)
					sensor.Close();

				sensor = null;
			}
		}

		#endregion Life cycle

		#region Private methods

		private void acquireBody()
		{
			BodyFrame frame = bodyReader.AcquireLatestFrame();
			if (frame != null)
			{
				frame.GetAndRefreshBodyData(bodies);
				frame.Dispose();

				Body body = Body;
				if (body != null)
				{
					hasTotalTracking = true;

					for (JointType type = JointType.SpineBase; type < JointType.ThumbRight; type++)
					{
						if (body.Joints[type].TrackingState == TrackingState.NotTracked)
						{
							hasTotalTracking = false;
							break;
						}
					}

					isInsideLimits = body.Joints[JointType.SpineBase].Position.Z > zLimit;
				}
			}
		}

		private void acquireFace()
		{
			if (Body == null)
				return;

			if (!faceSource.IsTrackingIdValid || faceSource.TrackingId != Body.TrackingId)
			{
				faceSource.TrackingId = Body.TrackingId;
			}

			using (FaceFrame frame = faceReader.AcquireLatestFrame())
			{
				if (frame == null || frame.TrackingId == 0)
					return;
				
				FaceFrameResult face = frame.FaceFrameResult;

				if (face == null) return;
				{
					var faceBox = face.FaceBoundingBoxInColorSpace;

					if (isValidFace(faceBox))
					{
						trackedFace = face;
					}
				}
			}
		}

		private bool isValidFace(RectI bounds)
		{
			return  (bounds.Right - bounds.Left) > 0 &&
					(bounds.Bottom - bounds.Top) > 0 &&
					bounds.Right <= displayWidth &&
					bounds.Bottom <= displayHeight;
		}

		private void acquireHDFace()
		{
			if (!hdFaceSource.IsTrackingIdValid)
			{
				if (Body != null)
					hdFaceSource.TrackingId = Body.TrackingId;
				
				return;
			}

			using (HighDefinitionFaceFrame frame = hdFaceReader.AcquireLatestFrame())
			{
				if (frame != null)
				{
					frame.GetAndRefreshFaceAlignmentResult(trackedHDFace);
				}
			}
		}

		#endregion Private methods

	}

}