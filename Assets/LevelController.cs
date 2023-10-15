using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;

    public void Init()
    {
        SoundController.Instance.PlayMusic(music);
        parts[0].StartPart();
    }

    [SerializeField]
    private LevelPart[] parts;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        RenderGizmo();
    }
#endif

    public void RenderGizmo()
    {
        parts = GetComponentsInChildren<LevelPart>();
        foreach (var part in parts)
        {
            part.RenderGizmo();
        }
    }
}
