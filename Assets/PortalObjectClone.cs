using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalObjectClone : MonoBehaviour
{

    PortalDetector portalObject;
    Portal portal;
    Vector3 velocity;
    Vector3 forward;
    MeshRenderer render;

    public void Init(PortalDetector pd, Portal p)
    {
        portalObject = pd;
        portal = p;
        velocity = portal.transform.InverseTransformDirection(portalObject.Rb.velocity);
        forward = portal.transform.InverseTransformDirection(portalObject.transform.forward);
        render = GetComponent<MeshRenderer>();
        render.materials[0].SetFloat("_Slice", 1f);
    }

    private void Update()
    {
        Vector3 v = velocity * Time.deltaTime;
        v = portal.transform.TransformDirection(v);
        transform.rotation = Quaternion.LookRotation(portal.transform.TransformDirection(forward));
        transform.position += v;
        render.materials[0].SetVector("_Pos", portal.transform.position);
        render.materials[0].SetVector("_Dir", portal.transform.up);

        if (!render.bounds.Intersects(portal.Collider.bounds))
        {
            Destroy(gameObject);
        }
    }
}
