using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDynamicRotate : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform start;
    [SerializeField]
    private Transform end;

    private Quaternion rotation;

    private void Update()
    {
        target.rotation = Quaternion.Lerp(target.rotation, rotation, Time.deltaTime * 5);
    }

    public void OnValueChange(float t)
    {
        rotation = Quaternion.Lerp(start.rotation, end.rotation, t);
    }
}
