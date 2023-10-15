using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class RigInputWrapper : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    private Vector3 lastCamPos;
    [SerializeField]
    private Transform cameraOffset;

    [SerializeField]
    private PlayerHandController rightHand;
    [SerializeField]
    private PlayerHandController leftHand;

    XROrigin xrOrigin;

    private ControllerInputs leftControllerInputs;
    private ControllerInputs rightControllerInputs;
    private FixedSizedV3Queue headPosTracking;

    public ControllerInputs LeftControllerInputs { get => leftControllerInputs; }
    public ControllerInputs RightControllerInputs { get => rightControllerInputs; }
    public FixedSizedV3Queue HeadPosTracking { get => headPosTracking; }
    public Camera Cam { get => cam; }
    public PlayerHandController LeftHand { get => leftHand; }
    public PlayerHandController RightHand { get => rightHand; }

    public void Init()
    {
        lastCamPos = cam.transform.position;
         
        gameObject.SetActive(true);
        cam.transform.parent = cameraOffset;
        leftControllerInputs = new ControllerInputs(9, 2);
        rightControllerInputs = new ControllerInputs(9, 2);
        headPosTracking = new FixedSizedV3Queue(60,0,true);
        xrOrigin = GetComponent<XROrigin>();
    }

    public void RecenterCam()
    {
        Vector3 v = transform.position - cam.transform.position;
        v.y = 0;

        xrOrigin.CameraFloorOffsetObject.transform.localPosition = v;
    }

    public void UpdateTracking()
    {
        Vector3 dir = cam.transform.position - lastCamPos;
        lastCamPos = cam.transform.position;

        headPosTracking.Enqueue(dir);
        leftControllerInputs.UpdateTrack();
        rightControllerInputs.UpdateTrack();
    }

    public void UpdateLeftHand(Vector3 pos, Quaternion rot)
    {
        leftControllerInputs.posDir = pos - leftControllerInputs.localPos;
        leftControllerInputs.localPos = pos;
        leftControllerInputs.globalPos = pos + transform.position;
        leftControllerInputs.rot = rot * transform.rotation;
    }

    public void UpdateLeftHand(ref InteractionState select, ref InteractionState active, ref InteractionState one, ref InteractionState two, Vector2 jst)
    {
        leftControllerInputs.active = active;
        leftControllerInputs.select = select;
        rightControllerInputs.button_one = one;
        rightControllerInputs.button_two = two;
        leftControllerInputs.joystick = jst;
    }

    public void UpdateRightHand(Vector3 pos, Quaternion rot)
    {
        rightControllerInputs.posDir = pos - rightControllerInputs.localPos;
        rightControllerInputs.localPos = pos;
        rightControllerInputs.globalPos = pos + transform.position;
        rightControllerInputs.rot = rot * transform.rotation;
    }

    public void UpdateRightHand(ref InteractionState select, ref InteractionState active, ref InteractionState one, ref InteractionState two, Vector2 jst)
    {
        rightControllerInputs.active = active;
        rightControllerInputs.select = select;
        rightControllerInputs.button_one = one;
        rightControllerInputs.button_two = two;
        rightControllerInputs.joystick = jst;
    }
}

public class ControllerInputs
{
    public InteractionState select;
    public InteractionState active;
    public InteractionState button_one;
    public InteractionState button_two;
    public Vector3 localPos;
    public Vector3 globalPos;
    public Vector3 posDir;
    public Quaternion rot;
    public FixedSizedV3Queue deltaPosTracking;
    public Vector2 joystick;

    public ControllerInputs(int frameTrackCount = 20, int frameLimitCount = 0)
    {
        deltaPosTracking = new FixedSizedV3Queue(frameTrackCount, frameLimitCount);
    }

    public void UpdateTrack()
    {
        deltaPosTracking.Enqueue(posDir);
    }
}

public class FixedSizedV3Queue
{
    readonly LinkedList<Vector3> queue = new LinkedList<Vector3>();
    readonly LinkedList<Vector3> queueLimited = new LinkedList<Vector3>();
    Vector3 avrg = Vector3.zero;
    Vector3 lastEnque = Vector3.zero;

    Vector3 avrgIndexLimit = Vector3.zero;
    bool useLimit = false;

    bool trackDistance = false;
    float totalDistance = 0;

    public int Size { get; private set; }
    public int Limit { get; private set; }
    public Vector3 Sum { get => avrg; }
    public float TotalDistance { get => totalDistance; }

    public FixedSizedV3Queue(int size, int limit = 0, bool track = false)
    {
        Size = size;
        Limit = limit;
        useLimit = limit != 0;
        avrg = Vector3.zero;
        avrgIndexLimit = Vector3.zero;
        trackDistance = track;
    }

    public void Enqueue(Vector3 obj)
    {
        queue.AddFirst(obj);
        avrg += obj;

        if (trackDistance)
        {
            totalDistance += (obj - lastEnque).magnitude;
            lastEnque = obj;
        }

        while (queue.Count > Size)
        {
            Vector3 outObj = queue.Last.Value;
            avrg -= outObj;
            queue.RemoveLast();

            if (trackDistance)
            {
                totalDistance -= (queue.Last.Value - outObj).magnitude;
            }
        }

        if (useLimit) 
        {
            queueLimited.AddFirst(obj);
            avrgIndexLimit += obj;

            while (queueLimited.Count > Limit)
            {
                Vector3 outObj = queueLimited.Last.Value;
                avrgIndexLimit -= outObj;
                queueLimited.RemoveLast();
            }
        }
    }

    public Vector3 GetAVRG()
    {
        return (avrg / queue.Count);
    }

    public Vector3 GetAVRGLimited()
    {
        return ((avrg - avrgIndexLimit) / (queue.Count - queueLimited.Count));
    }
}