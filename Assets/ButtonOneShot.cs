using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField]
    private UnityEvent onStepStart;

    [SerializeField]
    private UnityEvent onStepEnd;

    public void Click()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(CRT_Clicked());
    }

    IEnumerator CRT_Clicked()
    {
        SoundController.Instance.PlaySFX(SFXController.Sounds.Button_push, transform.position + Vector3.up);

        meshRenderer.material = materialOn;
        cable.SetActive(true, cableMaterialOn);
        tween.Play();
        UpdateState(true);

        onStepStart.Invoke();

        yield return new WaitForSeconds(time);

        cable.SetActive(false, cableMaterialOn);
        meshRenderer.material = materialOff;
        UpdateState(false);

        onStepEnd.Invoke();
    }
}
