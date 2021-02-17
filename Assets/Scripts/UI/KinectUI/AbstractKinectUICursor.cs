using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public abstract class AbstractKinectUICursor : MonoBehaviour {

    [SerializeField]
    protected KinectUIHandType handType;
    protected KinectInputData data;
    protected Image image;

    public virtual void Start()
    {
        Setup();
    }

    protected void Setup()
    {
        data = KinectInputModule.instance.GetHandData(handType);
        
        // Make sure we dont block raycasts
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().interactable = false;

        image = GetComponent<Image>();

    }

    public virtual void Update()
    {
        if (data == null || !data.IsTracking)
            return;
        ProcessData();
    }

    public abstract void ProcessData();
}
