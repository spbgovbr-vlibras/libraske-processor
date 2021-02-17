using UnityEngine.UI;

public class KinectUIWaitCursor : AbstractKinectUICursor
{

    public override void Start()
    {

        base.Start();

        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillAmount = 0f;
    }

    public override void ProcessData()
    {

        //transform.position = data.GetHandScreenPosition();
        

        if(data.IsHovering)
        {
            image.fillAmount = data.WaitOverAmount;
        }
        else
        {
            image.fillAmount = 0f;
        }
    }
}
