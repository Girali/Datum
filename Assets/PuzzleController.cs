using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleController : PuzzleStep
{
    [SerializeField]
    protected PuzzleStep[] steps;

    [SerializeField]
    protected UnityEvent onComplet;
    [SerializeField]
    protected UnityEvent onUncomplet;

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        foreach (var step in steps)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(step.transform.position, transform.position);
        }
    }
#endif

    public virtual void NotifyStateChange(PuzzleStep puzzleStep)
    {
        bool c = true;

        foreach(var step in steps) 
        { 
            if(!step.IsCompleted)
            {
                if (isCompleted)
                {
                    onUncomplet.Invoke();
                }

                c = false;
                break;
            }
        }

        UpdateState(c);

        if (isCompleted)
        {
            onComplet.Invoke();
        }
    }
}
