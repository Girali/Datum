using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    public AudioClip emptyClip;
    public GameObject audioSource;
    private AudioSource[] audioSources;

    private void Awake()
    {
        audioSources = audioSource.GetComponents<AudioSource>();
    }

    private AudioSource FindFreeSource()
    {
        foreach (AudioSource item in audioSources)
        {
            if (!item.isPlaying)
                return item;
        }

        return null;
    }

    public void PlayEmptyClip()
    {
        AudioSource s = FindFreeSource();
        s.volume = Random.Range(0.9f, 1f);
        s.pitch = Random.Range(0.9f, 1.1f);
        s.clip = emptyClip;
        s.Play();
    }
}
