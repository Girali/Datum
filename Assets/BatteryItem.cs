using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BatteryItem : InteractablePhysicObject
{
    public BatteryType batteryType;
    protected BatterySocket batterySocket = null;
    [SerializeField]
    protected Material materialOn;
    [SerializeField]
    protected Material materialOff;


    public BatterySocket BatterySocket
    {
        get
        {
            return batterySocket;
        }
    }

    protected override void UpdateMaterials()
    {
        if (batterySocket != null)
        {
            outlinedMat = new Material[] { materialOn, outline };
            normalMat = new Material[] { materialOn };
        }
        else
        {
            outlinedMat = new Material[] { materialOff, outline };
            normalMat = new Material[] { materialOff };
        }

        meshRenderer.materials = normalMat;
    }

    public void PlaceObject(BatterySocket bs)
    {
        InteruptInteract();
        batterySocket = bs;
        StartCoroutine(CRT_Place(bs));
    }

    public override void StartInteract(GameObject player, ControllerInputs ci, PlayerHandController phc)
    {
        base.StartInteract(player, ci, phc);

        if(batterySocket != null)
        {
            rb.isKinematic = false;
            batterySocket = null;
            UpdateMaterials();
        }
    }

    private IEnumerator CRT_Place(BatterySocket bs)
    {
        interactable = false;

        bool placed = false;
        bool rotated = false;

        rb.isKinematic = true;

        while (!placed || !rotated)
        {
            transform.position = Vector3.Lerp(transform.position ,bs.Socket.position, 0.05f);
            transform.rotation = Quaternion.Lerp(transform.rotation, bs.Socket.rotation, 0.05f);

            float d = Vector3.Distance(transform.position, bs.Socket.position);
            float a = Vector3.Dot(transform.forward, bs.Socket.forward);

            if (d < 0.05f)
                placed = true;

            if (a > 0.98f)
                rotated = true;

            yield return null;
        }

        transform.position = bs.Socket.position;
        transform.rotation = bs.Socket.rotation;

        interactable = true;
        UpdateMaterials();
    }
}
