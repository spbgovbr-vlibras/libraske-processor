using UnityEngine;
using UnityEngine.EventSystems;
using Windows.Kinect;
using LAViD.LibrasKe;

public enum KinectUIClickGesture
{
    HandState, Push, WaitOver
}
public enum KinectUIHandType
{
    Right, Left
}

[System.Serializable]
public class KinectInputData
{
    // Which hand we are tracking
    public KinectUIHandType trackingHandType = KinectUIHandType.Right;
    // We can normalize camera z position with this
    public float handScreenPositionMultiplier = 5f;
    // Is hand in pressing condition
    private bool _isPressing;//, _isHovering;
    // Hovering Gameobject, needed for WaitOver like clicking detection
    private GameObject _hoveringObject;
    // Joint type, we need it for getting body's hand world position
    public JointType handType
    {
        get
        {
            if (trackingHandType == KinectUIHandType.Right)
                return JointType.HandRight;
            else
                return JointType.HandLeft;
        }
    }
    // Hovering Gameobject getter setter, needed for WaitOver like clicking detection
    public GameObject HoveringObject
    {
        get { return _hoveringObject; }
        set
        {
            if (value != _hoveringObject)
            {
                HoverTime = Time.time;
                _hoveringObject = value;
                if (_hoveringObject == null) return;
                if (_hoveringObject.GetComponent<KinectUIWaitOverButton>())
                    ClickGesture = KinectUIClickGesture.WaitOver;
                else
                    ClickGesture = KinectUIClickGesture.HandState;
                WaitOverAmount = 0f;
            }
        }
    }
    public HandState CurrentHandState { get; private set; }
    public KinectUIClickGesture ClickGesture { get; private set; }
    public bool IsTracking { get; private set; }
    public bool IsHovering { get; set; }
    public bool IsDraging { get; set; }

    public bool IsPressing
    {
        get { return _isPressing; }
        set
        {
            _isPressing = value;
            if (_isPressing)
                TempHandPosition = HandPosition;
        }
    }
    
    public Vector3 HandPosition { get; private set; }
    public Vector3 TempHandPosition { get; private set; }
    public float HoverTime { get; set; }
    public float WaitOverAmount { get; set; }

    private Vector3 spineMid;

    // chamar para cada mao
    public void UpdateComponent(Body body)
    {

        if (body.Joints[handType].TrackingState != TrackingState.NotTracked)
        {
            
            HandPosition = GetVector3FromJoint(body.Joints[handType]);
            spineMid = GetVector3FromJoint(body.Joints[JointType.SpineMid]);
            CurrentHandState = GetStateFromJointType(body, handType);
            IsTracking = true;
        }
 
    }

    // Converts hand position to screen coordinates
    public Vector3 GetHandScreenPosition()
    {
        //return Camera.main.WorldToScreenPoint(new Vector3(HandPosition.x, HandPosition.y, HandPosition.z - handScreenPositionMultiplier));
        //return Camera.main.WorldToScreenPoint(HandPosition.WithZ(0));

        //Vector3 lowerLeft = new Vector3(0.6f, 0.6f, 0);
        Vector3 lowerLeft = new Vector3(2f, 2f, 0);
        Vector3 dimension = new Vector3(4.2f, 4.2f, 0);

        Vector3 diff = HandPosition - spineMid - lowerLeft;
        /*
        //Debug.Log("smx: " + spineMid.x + ", smy: " + spineMid.y);
        //Debug.Log("hx: " + HandPosition.x + ", hy: " + HandPosition.y);
        //Debug.Log("dx: " + diff.x + ", dy: " + diff.y);

        //Debug.Log(GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x);
        Camera c = (Camera)GameObject.Find("Camera").GetComponent<Camera>();
        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        Debug.Log(canvas.sizeDelta.x + ", " + canvas.sizeDelta.y);

        return new Vector3(canvas.sizeDelta.x * (diff.x / dimension.x), canvas.sizeDelta.y * (diff.y / dimension.y), 0);
        //return new Vector3(GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x * (diff.x / dimension.x), GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y * (diff.y / dimension.y), 0);
        */
        
        return new Vector3(Screen.width * (diff.x / dimension.x), Screen.height * (diff.y / dimension.y), 0);
    }

    // Get hand state data from kinect body
    private HandState GetStateFromJointType(Body body, JointType type)
    {
        switch (type)
        {
            case JointType.HandLeft:
                return body.HandLeftState;
            case JointType.HandRight:
                return body.HandRightState;
            default:
                Debug.LogWarning("Please select a hand joint, by default right hand will be used!");
                return body.HandRightState;
        }
    }
    // Get Vector3 position from Joint position
    private Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {

        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);

    }
}

[AddComponentMenu("Kinect/Kinect Input Module")]
[RequireComponent(typeof(EventSystem))]
public class KinectInputModule : BaseInputModule
{
    public KinectInputData[] _inputData = new KinectInputData[0];

    [SerializeField]
    private float _scrollTreshold = .5f;
    [SerializeField]
    private float _scrollSpeed = 3.5f;
    [SerializeField]
    private float _waitOverTime = 2f;

    PointerEventData handPointerData;

    static KinectInputModule _instance = null;

    public static KinectInputModule instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(KinectInputModule)) as KinectInputModule;

                if (!_instance)
                {
                    if (EventSystem.current)
                    {
                        EventSystem.current.gameObject.AddComponent<KinectInputModule>();
                        Debug.LogWarning("Add Kinect Input Module to your EventSystem!");
                    }
                    else
                        Debug.LogWarning("Create your UI first");
                }
            }
            return _instance;
        }
    }

    public void TrackBody(Body body)
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            _inputData[i].UpdateComponent(body);
        }
    }

    private PointerEventData GetLookPointerEventData(Vector3 componentPosition)
    {
        if (handPointerData == null)
        {
            handPointerData = new PointerEventData(eventSystem);
        }

        handPointerData.Reset();
        handPointerData.delta = Vector2.zero;
        handPointerData.position = componentPosition;
        handPointerData.scrollDelta = Vector2.zero;
        eventSystem.RaycastAll(handPointerData, m_RaycastResultCache);
        handPointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();
        return handPointerData;
    }

    public override void Process()
    {
        ProcessHover();
        ProcessPress();
        ProcessDrag();
        ProcessWaitOver();
    }
    
    private void ProcessWaitOver()
    {
        for (int j = 0; j < _inputData.Length; j++)
        {
            if (!_inputData[j].IsHovering || _inputData[j].ClickGesture != KinectUIClickGesture.WaitOver) continue;
            _inputData[j].WaitOverAmount = (Time.time - _inputData[j].HoverTime) / _waitOverTime;
            if (Time.time >= _inputData[j].HoverTime + _waitOverTime)
            {
                PointerEventData lookData = GetLookPointerEventData(_inputData[j].GetHandScreenPosition());
                GameObject go = lookData.pointerCurrentRaycast.gameObject;
                ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.submitHandler);

                _inputData[j].HoverTime = Time.time;
            }
        }
    }

    private void ProcessDrag()
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            // if not pressing can't drag
            if (!_inputData[i].IsPressing)
                continue;

            if (Mathf.Abs(_inputData[i].TempHandPosition.x - _inputData[i].HandPosition.x) >
                _scrollTreshold || Mathf.Abs(_inputData[i].TempHandPosition.y - _inputData[i].HandPosition.y) > _scrollTreshold)
            {
                _inputData[i].IsDraging = true;
            }
            else
            {
                _inputData[i].IsDraging = false;
            }

            //Debug.Log("drag " + _inputData[i].IsDraging + " press " + _inputData[i].IsPressing);
            // If dragging use unit's eventhandler to send an event to a scrollview like component
            if (_inputData[i].IsDraging)
            {
                PointerEventData lookData = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
                eventSystem.SetSelectedGameObject(null);
                //Debug.Log("drag");
                GameObject go = lookData.pointerCurrentRaycast.gameObject;
                PointerEventData pEvent = new PointerEventData(eventSystem);
                pEvent.dragging = true;
                pEvent.scrollDelta = (_inputData[i].TempHandPosition - _inputData[i].HandPosition) * _scrollSpeed;
                pEvent.useDragThreshold = true;
                ExecuteEvents.ExecuteHierarchy(go, pEvent, ExecuteEvents.scrollHandler);
            }
        }
    }

    private void ProcessPress()
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            //Check if we are tracking hand state not wait over
            if (!_inputData[i].IsHovering || _inputData[i].ClickGesture != KinectUIClickGesture.HandState) continue;

            // If hand state is not tracked reset properties
            if (_inputData[i].CurrentHandState == HandState.NotTracked)
            {
                _inputData[i].IsPressing = false;
                _inputData[i].IsDraging = false;
            }
            
            // When we close hand and we are not pressing set property as pressed
            if (!_inputData[i].IsPressing && _inputData[i].CurrentHandState == HandState.Closed)
            {
                _inputData[i].IsPressing = true;
                //PointerEventData lookData = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
                //eventSystem.SetSelectedGameObject(null);
                //if (lookData.pointerCurrentRaycast.gameObject != null && !_inputData[i].IsDraging)
                //{
                //    GameObject go = lookData.pointerCurrentRaycast.gameObject;
                //    ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.pointerDownHandler);
                //}
            }
            
            // If hand state is opened and is pressed, make click action
            else if (_inputData[i].IsPressing && (_inputData[i].CurrentHandState == HandState.Open))//|| _inputData[i].CurrentHandState == HandState.Unknown))
            {
                //_inputData[i].IsDraging = false;
                PointerEventData lookData = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
                eventSystem.SetSelectedGameObject(null);
                if (lookData.pointerCurrentRaycast.gameObject != null && !_inputData[i].IsDraging)
                {
                    GameObject go = lookData.pointerCurrentRaycast.gameObject;
                    ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.submitHandler);
                    //ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.pointerUpHandler);
                }
                _inputData[i].IsPressing = false;
            }
        }
    }


    private void ProcessHover()
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            PointerEventData pointer = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
            var obj = handPointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointer, obj);
            // Hover update
            _inputData[i].IsHovering = obj != null ? true : false;
            //if (obj != null)
            _inputData[i].HoveringObject = obj;
        }
    }


    public KinectInputData GetHandData(KinectUIHandType handType)
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            if (_inputData[i].trackingHandType == handType)
                return _inputData[i];
        }
        return null;
    }
}



