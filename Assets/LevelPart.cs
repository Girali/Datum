using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelPart : MonoBehaviour
{
    private LevelController levelController;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        levelController = GetComponentInParent<LevelController>();
        levelController.RenderGizmo();
    }
#endif

    public void RenderGizmo()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.red;
        for (int i = 0; i < onStepStart.GetPersistentEventCount(); i++)
        {
            try
            {
                Component g = onStepStart.GetPersistentTarget(i) as Component;
                Gizmos.DrawSphere(g.transform.position, 0.15f);
                Gizmos.DrawLine(transform.position, g.transform.position);
            }
            catch
            {
                GameObject g = onStepStart.GetPersistentTarget(i) as GameObject;
                Gizmos.DrawSphere(g.transform.position, 0.15f);
                Gizmos.DrawLine(transform.position, g.transform.position);
            }
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < onStepEnd.GetPersistentEventCount(); i++)
        {
            try
            {
                Component g = onStepEnd.GetPersistentTarget(i) as Component;
                Gizmos.DrawSphere(g.transform.position, 0.1f);
                Gizmos.DrawLine(transform.position, g.transform.position);
            }
            catch
            {
                GameObject g = onStepEnd.GetPersistentTarget(i) as GameObject;
                Gizmos.DrawSphere(g.transform.position, 0.1f);
                Gizmos.DrawLine(transform.position, g.transform.position);
            }
        }
    }


    [SerializeField]
    private UnityEvent onStepStart;

    [SerializeField]
    private UnityEvent onStepEnd;


    public void StartPart()
    {
        onStepStart.Invoke();
    }

    public void EndPart()
    {
        onStepEnd.Invoke();
    }
}
