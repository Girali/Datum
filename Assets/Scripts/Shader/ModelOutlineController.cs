using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelOutlineController : MonoBehaviour
{
    [SerializeField]
    private Renderer[] renders;
    [SerializeField]
    private Material mat;

    bool additionalMaterialApplied = false;

    public void ShowHide(bool b)
    {
        if (b)
            SetAdditionalMaterial(mat);
        else
            ClearAdditionalMaterial();
    }

    private void SetAdditionalMaterial(Material newMaterial)
    {
        if (additionalMaterialApplied)
        {
            Debug.LogWarning("Tried to add additional material even though it was already added on " + name);
            return;
        }

        foreach (Renderer r in renders)
        {
            Material[] materialsArray = new Material[(r.materials.Length + 1)];
            r.materials.CopyTo(materialsArray, 0);
            materialsArray[materialsArray.Length - 1] = newMaterial;
            r.materials = materialsArray;
        }

        additionalMaterialApplied = true;
    }

    private void ClearAdditionalMaterial()
    {
        if (!additionalMaterialApplied)
        {
            Debug.LogWarning("Tried to delete additional material even though none was added before on " + name);
            return;
        }

        foreach (Renderer r in renders)
        {
            Material[] materialsArray = new Material[(r.materials.Length - 1)];
            for (int i = 0; i < r.materials.Length - 1; i++)
            {
                materialsArray[i] = r.materials[i];
            }
            r.materials = materialsArray;
        }

        additionalMaterialApplied = false;
    }
}
