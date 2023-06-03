using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverFloat : MonoBehaviour
{
#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        RenderView();
    }
#endif

    protected virtual void RenderView()
    {
        cableView?.RenderView();
    }

    [SerializeField]
    private CableView cableView;
    [SerializeField]
    private Material cableActiveMat;

    [SerializeField]
    private UnityEvent<float> onValueChange;

    public void OnValueChange(float f)
    {
        onValueChange.Invoke(f);

        if (Mathf.Approximately(f, 0))
        {
            cableView.SetActive(false, cableActiveMat);
        }
        else
        {
            cableView.SetActive(true, cableActiveMat);
        }
    }
}
