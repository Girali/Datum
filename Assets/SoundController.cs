using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    private DialogueController dialogueController;
    [SerializeField]
    private SFXController sfxController;
    [SerializeField]
    private MusicController musicController;

    private static SoundController _instance;
    public static SoundController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundController>();
            return _instance;
        }
    }

#region Utility
    public static IEnumerator CRT_FadeIn(AudioSource audioSource, float time)
    {
        float t = 0;
        float inter = (1f / 60f) * time;

        while (t < time)
        {
            t += inter;
            audioSource.volume = t / time;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = 1f;
    }

    public static IEnumerator CRT_FadeOut(AudioSource audioSource, float time)
    {
        float t = time;
        float inter = (1f / 60f) * time;

        while (t > 0)
        {
            t -= inter;
            audioSource.volume = t / time;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = 0f;
    }

    public AudioMixerGroup musicAudioMixerGroup;
    public AudioMixerGroup sfxAudioMixerGroup;
    public AudioMixerGroup dialogueAudioMixerGroup;
#endregion

    public void PlayDialogue(AudioClip clip)
    {
        dialogueController.Play(clip);
    }

    public void PlaySFX(SFXController.Sounds c, Vector3 pos)
    {
        sfxController.Play(c, pos);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicController.Play(clip);
    }
}
