using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RigInputWrapper : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform cameraOffset;

    private ControllerInputs leftControllerInputs;
    private ControllerInputs rightControllerInputs;
    private FixedSizedQueue headPosTracking;

    public ControllerInputs LeftControllerInputs { get => leftControllerInputs; }
    public ControllerInputs RightControllerInputs { get => rightControllerInputs; }
    public FixedSizedQueue HeadPosTracking { get => headPosTracking; }

    public void Init()
    {
        gameObject.SetActive(true);
        cam.transform.parent = cameraOffset;
        leftControllerInputs = new ControllerInputs();
        rightControllerInputs = new ControllerInputs();
        headPosTracking = new FixedSizedQueue(10);
    }

    public void UpdateTracking()
    {
        headPosTracking.Enqueue(cam.transform.position);
        leftControllerInputs.UpdateTrack();
        rightControllerInputs.UpdateTrack();
    }

    public void UpdateLeftHand(Vector3 pos, Quaternion rot)
    {
        leftControllerInputs.pos = pos;
        leftControllerInputs.rot = rot;
    }

    public void UpdateLeftHand(ref InteractionState select, ref InteractionState active, ref InteractionState ui)
    {
        leftControllerInputs.active = active;
        leftControllerInputs.select = select;
        leftControllerInputs.ui = ui;
    }

    public void UpdateRightHand(Vector3 pos, Quaternion rot)
    {
        rightControllerInputs.pos = pos;
        rightControllerInputs.rot = rot;
    }

    public void UpdateRightHand(ref InteractionState select, ref InteractionState active, ref InteractionState ui)
    {
        rightControllerInputs.active = active;
        rightControllerInputs.select = select;
        rightControllerInputs.ui = ui;
    }
}

public class ControllerInputs
{
    public InteractionState select;
    public InteractionState active;
    public InteractionState ui;
    public Vector3 pos;
    public Quaternion rot;
    public FixedSizedQueue deltaPosTracking;

    public ControllerInputs(int frameTrackCount = 20)
    {
        deltaPosTracking = new FixedSizedQueue(frameTrackCount);
    }

    public void UpdateTrack()
    {
        deltaPosTracking.Enqueue(pos);
    }
}

public class FixedSizedQueue
{
    readonly LinkedList<Vector3> queue = new LinkedList<Vector3>();
    Vector3 avrg = Vector3.zero;

    public int Size { get; private set; }
    public Vector3 Sum { get => avrg; }

    public FixedSizedQueue(int size)
    {
        Size = size;
        avrg = Vector3.zero;
    }

    public void Enqueue(Vector3 obj)
    {
        queue.AddFirst(obj);
        avrg += obj;

        while (queue.Count > Size)
        {
            Vector3 outObj = queue.Last.Value;
            avrg -= outObj;
            queue.RemoveLast();
        }
    }

    public Vector3 GetAVRG()
    {
        return (avrg / queue.Count);
    }
}