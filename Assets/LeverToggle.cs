using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverToggle : PuzzleStep
{
    protected override void RenderView()
    {
        base.RenderView();
        cableView?.RenderView();
    }

    private float activationPoint = 0.9f;

    [SerializeField]
    private Material materialOn;
    [SerializeField]
    private Material materialOff;

    [SerializeField]
    private MeshRenderer render;

    [SerializeField]
    private CableView cableView;
    [SerializeField]
    private Material cableActiveMat;

    private void Awake()
    {
        UpdateView();
    }

    protected override void UpdateState(bool state)
    {
        base.UpdateState(state);

        cableView.SetActive(isCompleted, cableActiveMat);

        UpdateView();
    }

    void UpdateView()
    {
        if (isCompleted)
        {
            render.material = materialOn;
        }
        else
        {
            render.material = materialOff;
        }
    }
     
    public void UpdateState(float t)
    { 
        if (isCompleted == false)
        {
            if (t >= activationPoint)
            {
                UpdateState(true);
            }
        }
        else
        {
            if (t < activationPoint)
            {
                UpdateState(false);
            }
        }
    }
}
