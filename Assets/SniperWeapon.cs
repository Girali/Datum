using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperWeapon : GunWeapon
{
    public LineRenderer lineRenderer;
    public LayerMask layerMask;
    
    private void Update()
    {
        lineRenderer.SetPosition(0, muzzle.transform.position);

        Vector3 endPos = muzzle.transform.position + (muzzle.transform.forward * 100f);

        RaycastHit hit;
        if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out hit, 250f,layerMask))
        {
            endPos = hit.point;
        }
        
        lineRenderer.SetPosition(1, endPos);
    }
}
