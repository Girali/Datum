using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonEnemy : Enemy
{
    public Transform head;
    
    protected override void Fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, maxDistance, layerMask))
        {
            if (hit.transform.gameObject.layer == player.gameObject.layer)
            {
                base.Fire();
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        Vector3 dir = (player.position) - muzzle.position;

        head.rotation = Quaternion.LookRotation(dir);
        
        if (dir.magnitude < maxDistance)
        {
            if (Time.time > nextFire)
            {
                Fire();
            }
        }
    }
}
