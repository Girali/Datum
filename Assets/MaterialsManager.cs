using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour
{
    [SerializeField]
    private Material[] materialsOn;
    [SerializeField]
    private Material[] materialsOff;

    private MeshRenderer meshRenderer;
    [SerializeField]
    private bool isOn = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetMaterial(isOn);
    }

    public void SetMaterial(bool b)
    {
        isOn = b;

        if (b)
            meshRenderer.materials = materialsOn;
        else
            meshRenderer.materials = materialsOff;
    }
}
