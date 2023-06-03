using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class InteractablePhysicObject : InteractableOutlined
{
    protected Rigidbody rb;
    protected bool locked = false;
    protected bool lerp = false;
    protected float maxDist = 5f;
    protected float lerpSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool IsKinematic
    {
        get
        {
            return rb.isKinematic;
        }

        set
        {
            rb.isKinematic = value;
        }
    }

    public void InteruptInteract()
    {
        if(playerHandController != null)
        {
            playerHandController.InteruptCurrentInteract();
        }
    }

    public override void StartInteract(GameObject player, ControllerInputs hand, PlayerHandController phc)
    {
        base.StartInteract(player, hand, phc);
        lerp = true;
    }

    public override void Interacting(GameObject player, ControllerInputs controllerInputs)
    {
        base.Interacting(player, controllerInputs);

        if (lerp)
        {
            if ((controllerInputs.globalPos - rb.transform.position).magnitude > 0.1f)
            {
                float t = (controllerInputs.globalPos - rb.transform.position).magnitude / maxDist;
                rb.velocity = (controllerInputs.globalPos - rb.transform.position) * Mathf.Lerp(lerpSpeed, 3f, t);
                rb.AddTorque((rb.transform.rotation * Quaternion.Inverse(controllerInputs.rot)).eulerAngles);
            }
            else
            {
                rb.velocity = Vector3.zero;
                locked = true;
                lerp = false;
                rb.isKinematic = true;
            }
        }

        if (locked)
        {
            rb.transform.position = controllerInputs.globalPos;
            rb.transform.rotation = controllerInputs.rot;
        }
    }

    public override void EndInteract(GameObject player, ControllerInputs controllerInputs)
    {
        base.EndInteract(player, controllerInputs);

        if (lerp)
        {
            rb.velocity += controllerInputs.deltaPosTracking.GetAVRGLimited() * 60f * 2f;
            lerp = false;
        }

        if(locked)
        {
            rb.isKinematic = false;
            rb.velocity = controllerInputs.deltaPosTracking.GetAVRGLimited() * 60f * 2f;
            locked = false;
        }
    }
}
