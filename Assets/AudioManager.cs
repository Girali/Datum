using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager
{
    private GameObject gameObject;
    private GameObject audioSourceObject;
    private List<AudioSource> audioSources;
    private List<AudioSource> audioBypassSources;
    private AudioMixerGroup audioMixerGroup;
    private AudioMixerGroup bypassGroup;
    public AudioManager(GameObject g, AudioMixerGroup a, AudioMixerGroup b)
    {
        gameObject = g;
        audioMixerGroup = a;
        audioSources = new List<AudioSource>();
        audioBypassSources = new List<AudioSource>();
        audioSourceObject = new GameObject("AudioSource");
        audioSourceObject.transform.parent = gameObject.transform;
        audioSourceObject.transform.localPosition = Vector3.zero;
        bypassGroup = b;
    }

    private AudioSource AddNewAudioSource(bool bypass)
    {
        AudioSource a = audioSourceObject.AddComponent<AudioSource>();
        if(bypass)
            audioBypassSources.Add(a);
        else
            audioSources.Add(a);

        a.outputAudioMixerGroup = bypass ? bypassGroup : audioMixerGroup;
        a.playOnAwake = false;
        a.spatialize = true;
        a.spatialBlend = 1f;
        return a; 
    }

    private AudioSource FindFreeAudioSource(bool b)
    {
        AudioSource a = null;
        if (b)
        {
            foreach (AudioSource audioSource in audioBypassSources)
            {
                if (!audioSource.isPlaying)
                {
                    a = audioSource;
                }
            }
        }
        else
        {
            foreach (AudioSource audioSource in audioSources)
            {
                if (!audioSource.isPlaying)
                {
                    a = audioSource;
                }
            }
        }


        if (a == null)
        {
            a = AddNewAudioSource(b);
        }

        return a;
    }

    public void PlaySound(AudioClip c, Vector2? volumeRange = null, Vector2? pitchRange = null, bool bypass = false,
        Vector3? offset = null)

    {
        Vector2 volume = volumeRange ?? new Vector2(1, 1);
        Vector2 pitch = pitchRange ?? new Vector2(1, 1);
        Vector3 o = offset ?? Vector3.zero;

        AudioSource a = FindFreeAudioSource(bypass);
        a.volume = Random.Range(volume.x, volume.y);
        a.pitch = Random.Range(pitch.x, pitch.y);
        a.transform.localPosition = o;
        a.PlayOneShot(c);
    }
}
