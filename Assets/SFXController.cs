using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class SFXController : MonoBehaviour
{
    private GameObject player;
    private GameObject sources;
    private List<AudioSource> audioSources = new List<AudioSource>();

    [SerializeField]
    private SoundPair[] sounds = new SoundPair[0];

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sources = new GameObject("SFX Sources");
    }

    private AudioSource FindFreeSource()
    {
        foreach (AudioSource s in audioSources)
        {
            if (!s.isPlaying)
                return s;
        }

        GameObject source = new GameObject("Source_" + audioSources.Count);
        AudioSource audioSource = source.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SoundController.Instance.sfxAudioMixerGroup;
        audioSources.Add(audioSource);
        audioSource.volume = 1f;
        audioSource.spatialBlend = 1f;

        return audioSource;
    }

    private AudioClip FindClip(Sounds s)
    {
        foreach (SoundPair sp in sounds)
        {
            if (sp.sound == s)
                return sp.clip;
        }

        return null;
    }

    public void Play(Sounds s , Vector3 pos)
    {
        AudioClip ac = FindClip(s);
        if(ac != null)
        {
            AudioSource asrc = FindFreeSource();
            asrc.PlayOneShot(ac);
            asrc.transform.position = pos;
        }
    }

    public enum Sounds
    {
        Click,
        Sucess
    }

    [System.Serializable]
    public class SoundPair
    {
        public Sounds sound;
        public AudioClip clip;
    }
}
