using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;

public class PuzzleToggle : PuzzleController
{
    protected override void RenderView()
    {
        base.RenderView();
        cableViewOut?.RenderView();
    }

    [SerializeField]
    private Operation operation;

    [SerializeField]
    private CableView cableViewIn;
    [SerializeField]
    private CableView cableViewOut;

    [SerializeField]
    private Material materialOn;
    [SerializeField]
    private Material materialOff;
    [SerializeField]
    private MeshRenderer meshRenderer;

    public enum Operation
    {
        And,
        Or,
        BitMemory
    }

    public override void NotifyStateChange(PuzzleStep puzzleStep)
    {
        switch (operation)
        {
            case Operation.And:
                And(puzzleStep);
                break;
            case Operation.Or:
                Or(puzzleStep);
                break;
            case Operation.BitMemory:
                OneBitMemory(puzzleStep);
                break;
            default:
                break;
        }


        if (isCompleted)
        {
            onComplet.Invoke();
            meshRenderer.material = materialOn;
            cableViewOut.SetActive(true, cableViewIn.On);
        }
        else
        {
            onUncomplet.Invoke();
            meshRenderer.material = materialOff;
            cableViewOut.SetActive(false, cableViewIn.On);
        }
    }


    public void And(PuzzleStep puzzleStep)
    {
        bool b = true;

        foreach (PuzzleStep p in steps)
        {
            if (p.IsCompleted == false)
            {
                b = false;
                break;
            }
        }

        UpdateState(b);
    }

    public void Or(PuzzleStep puzzleStep)
    {
        bool b = false;

        foreach (PuzzleStep p in steps)
        {
            if (p.IsCompleted)
            {
                b = true;
                break;
            }
        }

        UpdateState(b);
    }

    public void OneBitMemory(PuzzleStep puzzleStep)
    {
        if (puzzleStep.IsCompleted)
        {
            bool c = !isCompleted;
            UpdateState(c);
        }
    }
}