using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ButtonOneShot : PuzzleStep
{
    protected override void RenderView()
    {
        base.RenderView();
        cable?.RenderView();
    }

    [SerializeField]
    private Material materialOn;
    [SerializeField]
    private Material materialOff;

    [SerializeField]
    private CableView cable;
    [SerializeField]
    private Material cableMaterialOn;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Jun_TweenRuntime tween;

    [SerializeField]
    private float time = 0.5f;

    private Coroutine coroutine;

    public void Click()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(CRT_Clicked());
    }

    IEnumerator CRT_Clicked()
    {
        meshRenderer.material = materialOn;
        cable.SetActive(true, cableMaterialOn);
        tween.Play();
        UpdateState(true);

        yield return new WaitForSeconds(time);

        cable.SetActive(false, cableMaterialOn);
        meshRenderer.material = materialOff;
        UpdateState(false);
    }
}
