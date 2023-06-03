using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 speed;
    [SerializeField]
    private Transform lookAt;
    [SerializeField]
    private Transform center;
    [SerializeField]
    private float distance;
    private Vector3 pos;

    private void Awake()
    {
        pos = Quaternion.LookRotation(transform.position - center.position).eulerAngles;
        pos = new Vector3(pos.x % 360, pos.y % 360, pos.z % 360);
    }

    void Update()
    {
        pos += new Vector3(speed.x , speed.y , speed.z) * Time.deltaTime;
        pos = new Vector3(pos.x % 360, pos.y % 360, pos.z % 360);

        transform.position = center.position + (Quaternion.Euler(pos) * Vector3.forward * distance);
        transform.rotation =    Quaternion.LookRotation(transform.position - lookAt.position);
    }
}
