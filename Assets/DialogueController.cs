using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class DialogueController : MonoBehaviour
{
    private GameObject player;
    private GameObject source;
    private List<AudioSource> audioSources = new List<AudioSource>();

    public void Init()
    {
        player = Camera.main.gameObject;
        source = new GameObject("Dialogue Source");
    }

    private void Update()
    {
        source.transform.position = Vector3.Lerp(source.transform.position, player.transform.position + (Vector3.up * 4f) + (player.transform.forward * 3f), Time.deltaTime);
    }

    private AudioSource FindFreeSource()
    {
        foreach (AudioSource s in audioSources)
        {
            if (!s.isPlaying)
                return s;
        }

        AudioSource audioSource = source.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SoundController.Instance.dialogueAudioMixerGroup;
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
