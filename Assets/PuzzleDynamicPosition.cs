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

    [SerializeField]
    private float speed = 5;

    [SerializeField]
    private LerpType type = LerpType.Lerp;

    public enum LerpType
    {
        Const,
        Lerp
    }

    private void Awake()
    {
        position = start.position;
    }

    private void Update()
    {
        switch (type)
        {
            case LerpType.Const:
                target.position = Vector3.MoveTowards(target.position, position, Time.deltaTime * speed);
                break;
            case LerpType.Lerp:
                target.position = Vector3.Lerp(target.position, position, Time.deltaTime * speed);
                break;
            default:
                break;
        }
    }

    public void OnValueChange(float t)
    {
        position = Vector3.Lerp(start.position, end.position, t);
    }
}
