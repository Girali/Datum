using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableView : MonoBehaviour
{
    [SerializeField]
    private Material off;

    [SerializeField]
    private float offset = 0.01f;

    public Material On { get => on; }

    [FoldoutGroup("Optional")]
    [SerializeField]
    private Material on;

    [FoldoutGroup("Optional")]
    [SerializeField]
    private CableView[] cableViews = null;

    public void SetActive(bool active, Material on)
    {
        this.on = on;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (active)
        {
            lineRenderer.material = on;
        }
        else
        {
            lineRenderer.material = off;
        }

        if (cableViews != null)
        {
            foreach (CableView item in cableViews)
            {
                item.SetActive(active, on);
            }
        }
    }

    [Button("Gen")]
    public void CalculatePath()
    {
        int i = 0;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = transform.childCount;

        foreach (Transform t in transform)
        {
            lineRenderer.SetPosition(i, t.position + (t.forward * offset));
            i++;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        RenderView();
        if (cableViews != null)
        {
            foreach (CableView item in cableViews)
            {
                item.RenderView();
            }
        }
    }
#endif

    public void RenderView()
    {
        int i = 0;
        Transform temp = null;
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(t.position, t.position + t.forward);
            if (temp != null)
            {
                Gizmos.DrawLine(temp.position, t.position);
            }
            temp = t;
            i++;
        }
    }
}
