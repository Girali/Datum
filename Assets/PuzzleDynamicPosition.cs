using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDynamicPosition : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform start;
    [SerializeField]
    private Transform end;

    private Vector3 position;

    private void Update()
    {
        target.position = Vector3.Lerp(target.position, position, Time.deltaTime * 5);
    }

    public void OnValueChange(float t)
    {
        position = Vector3.Lerp(start.position, end.position, t);
    }
}
