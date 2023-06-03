using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FromToMovement : MonoBehaviour
{
    [SerializeField]
    private Transform pointA;
    [SerializeField]
    private Transform pointB;
    private float time;
    private float direction = 1;
    [SerializeField]
    private float speed;

    void Update()
    {
        time += direction * speed * Time.deltaTime;

        if (time > 1)
            direction = -1;

        if (time < 0)
            direction = 1;

        transform.position = Vector3.Lerp(pointA.position, pointB.position, time);
    }
}
