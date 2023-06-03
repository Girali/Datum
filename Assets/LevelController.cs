using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;

    private void Awake()
    {
        SoundController.Instance.PlayMusic(music);
    }

    private LevelPart[] parts;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        parts = GetComponentsInChildren<LevelPart>();
        RenderGizmo();
    }
#endif

    public void RenderGizmo()
    {
        Transform temp = null;
        foreach (var part in parts)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(part.transform.position, 0.5f);

            if(temp != null)
            {
                Gizmos.DrawLine(temp.position, part.transform.position);
            }

            temp = part.transform;
        }
    }
}
