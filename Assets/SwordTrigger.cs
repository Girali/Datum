using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    public SwordWeapon swordWeapon;
    
    private void OnTriggerEnter(Collider other)
    {
        swordWeapon.OnEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        swordWeapon.OnExit(other);
    }
}
