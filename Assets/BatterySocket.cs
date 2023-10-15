using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocket : PuzzleStep
{
    protected override void RenderView()
    {
        base.RenderView();
        cableView?.RenderView();
    }

    public BatteryType batteryType;

    [SerializeField]
    private Jun_TweenRuntime activeAnim;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material matOn;
    [SerializeField]
    private Material matOff;

    [SerializeField]
    private Transform socket;
    private BatteryItem batteryItem;

    [SerializeField]
    private ParticleSystem batteryParticles;
    [SerializeField]
    private CableView cableView;
    [SerializeField]
    private Material cableActiveMat;

    public Transform Socket
    {
        get { return socket; }
    }

    private void Awake()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (batteryItem == null)
        {
            meshRenderer.material = matOff;
            activeAnim.playType = Jun_TweenRuntime.PlayType.Deful;

            batteryParticles.Stop(true);
            cableView.SetActive(false, cableActiveMat);

            SoundController.Instance.PlaySFX(SFXController.Sounds.Object_release, transform.position);
        }
        else
        {
            meshRenderer.material = matOn;
            activeAnim.playType = Jun_TweenRuntime.PlayType.Loop;

            batteryParticles.Play(true);
            cableView.SetActive(true, cableActiveMat);

            SoundController.Instance.PlaySFX(SFXController.Sounds.Object_grab, transform.position);
        }

        if (!activeAnim.isPlaying)
            activeAnim.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (batteryItem == null)
        {
            BatteryItem item = other.attachedRigidbody?.GetComponent<BatteryItem>();
            if (item)
            {
                if (item.batteryType == batteryType)
                {
                    UpdateState(true);
                    batteryItem = item;
                    item.PlaceObject(this);
                    UpdateView();
                }
            }
        }
    }

    private void Update()
    {
        if (batteryItem != null)
        {
            float d = Vector3.Distance(batteryItem.transform.position, transform.position);

            if (d > 1.5f)
            {
                UpdateState(false);
                batteryItem = null;
                UpdateView();
            }
        }
    }
}

[Flags]
public enum BatteryType
{
    Red = 1,
    Green = 2,
    Blue = 4,
    Yellow = 8,

    Triangle = 16,
    Sqaure = 32,
    Circle = 64,
}
