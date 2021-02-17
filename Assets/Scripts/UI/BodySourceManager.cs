using UnityEngine;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour 
{
    
    private Body[] data = null;
    
    public Body[] GetData()
    {
        return data;
    }
    
    void Start ()
    {
        if(GameData.sensor == null)
        {
            GameData.sensor = KinectSensor.GetDefault();
        }

        if (GameData.sensor != null)
        {
            if (GameData.reader == null)
            {
                GameData.reader = GameData.sensor.BodyFrameSource.OpenReader();
            }
            
            if (!GameData.sensor.IsOpen)
            {
                GameData.sensor.Open();
            }
        }   
    }
    
    void Update () 
    {
        if (GameData.reader != null)
        {
            var frame = GameData.reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (data == null)
                {
                    data = new Body[GameData.sensor.BodyFrameSource.BodyCount];
                }
                
                frame.GetAndRefreshBodyData(data);
                
                frame.Dispose();
                frame = null;
            }
        }    
    }
    
    void OnApplicationQuit()
    {

        if (GameData.reader != null)
        {
            GameData.reader.Dispose();
            GameData.reader = null;
        }
        
        if (GameData.sensor != null)
        {
            if (GameData.sensor.IsOpen)
            {
                GameData.sensor.Close();
            }

            GameData.sensor = null;
        }
    }

}
