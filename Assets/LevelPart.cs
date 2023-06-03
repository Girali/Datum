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
