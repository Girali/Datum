using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class DialogueController : MonoBehaviour
{
    private GameObject player;
    private GameObject source;
    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        source = new GameObject("Dialogue Source");
    }

    private void Update()
    {
        source.transform.position = Vector3.Lerp(source.transform.position, player.transform.position + (Vector3.up * 5f), Time.deltaTime * 5f);
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
        audioSource.spatialBlend = 1f;

        return audioSource;
    }

    public void Play(AudioClip clip)
    {
        AudioSource s = FindFreeSource();
        s.PlayOneShot(clip);
    }
}
