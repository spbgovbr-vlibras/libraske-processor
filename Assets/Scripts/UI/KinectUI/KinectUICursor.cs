using UnityEngine;

public class KinectUICursor : AbstractKinectUICursor
{
    public Color normalColor = new Color(1f, 1f, 1f, 1f);
    public Color hoverColor = new Color(1f, 1f, 1f, 1f);
    public Color clickColor = new Color(1f, 1f, 1f, 1f);
    public Vector3 clickScale = new Vector3(.9f, .9f, 0f);

    private Vector3 initScale;

    public override void Start()
    {
        base.Start();
        initScale = transform.localScale;
        image.color = new Color(1f, 1f, 1f, 0f);
    }

    public override void ProcessData()
    {

        transform.position = Vector3.Slerp(transform.position, data.GetHandScreenPosition(), 0.5f);
        
        if (data.IsPressing)
        {
            image.color = clickColor;
            image.transform.localScale = clickScale;
            return;
        }
        if (data.IsHovering)
        {
            image.color = hoverColor;
        }
        else
        {
            image.color = normalColor;
        }

        image.transform.localScale = initScale;

    }
}
