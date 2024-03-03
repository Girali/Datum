using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public bool inside = false;

    private int count = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        count++;
        inside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        count--;
        if (count == 0)
            inside = false;
    }
}
