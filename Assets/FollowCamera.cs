using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform toFollow;
    public float speedLerpR = 0.5f;
    public float speedLerpT = 0.5f;
    
    private void Awake()
    {
        toFollow = new GameObject().transform;
        toFollow.parent = transform.parent;
        toFollow.position = transform.position;
        toFollow.rotation = transform.rotation;
        
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, toFollow.position , speedLerpT);
        transform.rotation = Quaternion.Lerp(transform.rotation, toFollow.rotation, speedLerpR);
    }
}
