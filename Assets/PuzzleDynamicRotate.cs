using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleDynamicRotate : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform start;
    [SerializeField]
    private Transform end;

    private Quaternion rotation;

    [SerializeField]
    private float speed = 5;

    private void Awake()
    {
        rotation = start.rotation;
    }

    private void Update()
    {
        target.rotation = Quaternion.Lerp(target.rotation, rotation, Time.deltaTime * speed);
    }

    public void OnValueChange(float t)
    {
        rotation = Quaternion.Lerp(start.rotation, end.rotation, t);
    }
}
