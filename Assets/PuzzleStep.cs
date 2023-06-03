using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PuzzleStep : MonoBehaviour
{
    [SerializeField]
    protected PuzzleController controller;
    protected bool isCompleted = false;

    public bool IsCompleted { get => isCompleted; }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        RenderView();
    }
#endif

    protected virtual void RenderView()
    {
        if (controller)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, controller.transform.position);
            controller.RenderView();
        }
    }

    protected virtual void UpdateState(bool state)
    {
        if (isCompleted != state)
        {
            isCompleted = state;
            if (controller)
            {
                controller.NotifyStateChange(this);
            }
        }
    }
}
