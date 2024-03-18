using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time = 2f;

    private void Awake()
    {
        Destroy(gameObject, time);
    }
}
