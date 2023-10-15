using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleViewManager : MonoBehaviour
{
    private Jun_TweenRuntime tween;
    private MaterialsManager materialsManager;
    private bool isActive = false;

    private void Awake()
    {
        tween = GetComponent<Jun_TweenRuntime>();
        materialsManager = GetComponent<MaterialsManager>();
    }

    public void SetActive(bool b)
    {
        isActive = b;
        materialsManager.SetMaterial(isActive);

        if (isActive)
        {
            tween.Play();
            SoundController.Instance.PlaySFX(SFXController.Sounds.Door_open, transform.position);
        }
        else
        {
            tween.Rewind();
            SoundController.Instance.PlaySFX(SFXController.Sounds.Door_close, transform.position);
        }
    }
}
