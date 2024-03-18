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

    public AudioMixer audioMixer;
    
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

    public void Init()
    {
        dialogueController.Init();
        sfxController.Init();
        musicController.Init();
    }

    #region Utility

    public void SetBulletTimeAudioEffect(float t)
    {
        audioMixer.SetFloat("Lowpass", Mathf.Lerp(900, 22000, t));
        audioMixer.SetFloat("Pitch", Mathf.Lerp(0.7f, 1f, t));
        audioMixer.SetFloat("MusicLowpass", Mathf.Lerp(1000f, 22000f, t));
    }
    
    public static IEnumerator CRT_FadeIn(AudioSource audioSource, float time)
    {
        float targetVolume = audioSource.volume;

        float t = 0;
        float inter = (1f / 60f) * time;

        while (t < time)
        {
            t += inter;
            audioSource.volume = (t / time) * targetVolume;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = targetVolume;
    }

    public static IEnumerator CRT_FadeOut(AudioSource audioSource, float time)
    {
        float targetVolume = audioSource.volume;

        float t = time;
        float inter = (1f / 60f) * time;

        while (t > 0)
        {
            t -= inter;
            audioSource.volume = (t / time) * targetVolume;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = 0f;
    }

    public AudioMixerGroup musicAudioMixerGroup;
    public AudioMixerGroup sfxAudioMixerGroup;
    public AudioMixerGroup dialogueAudioMixerGroup;
    public AudioMixerGroup bypassAudioMixerGroup;
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
