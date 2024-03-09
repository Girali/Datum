using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCamera : MonoBehaviour
{
    public Camera camera;
    private Vector3 pos;
    
    private void Update()
    {
        Vector3 v = camera.transform.localPosition;
        transform.localPosition = v;
    }
}
