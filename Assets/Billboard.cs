using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camera;
    
    private void Awake()
    {
        camera = GameObject.FindWithTag("MainCamera").transform;
    }

    private void Update()
    {
        transform.rotation = camera.rotation;
    }
}
