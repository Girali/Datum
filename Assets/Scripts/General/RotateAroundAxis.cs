using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAxis : MonoBehaviour
{
    public Vector3 rotation;
    public float speed = 5f;
    void Update()
    {
        transform.Rotate(rotation, Time.deltaTime * speed);
    }
}
