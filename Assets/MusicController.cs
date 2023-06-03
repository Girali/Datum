using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private GameObject player;
    private GameObject source;
    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        source = new GameObject("Music Source");
        source.transform.parent = player.transform;
        source.transform.localPosition = Vector3.zero;
    }

    private AudioSource FindFreeSource()
    {
        foreach (AudioSource s in audioSources)
        {
            if (!s.isPlaying)
                return s;
        }

        AudioSource audioSource = source.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SoundController.Instance.sfxAudioMixerGroup;
        audioSources.Add(audioSource);
        audioSource.volume = 1f;

        return audioSource;
    }

    public void Play(AudioClip clip)
    {
        AudioSource s = FindFreeSource();
        s.clip = clip;
        s.loop = true;
        s.Play();

        StartCoroutine(SoundController.CRT_FadeIn(s, 2f));
    }
}
