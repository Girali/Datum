using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioManager audioManager;
    public AudioClip grounded;
    public AudioSource wind;
    private PlayerPhysicController playerPhysicController;
    public Vector2 windVolumeRange = Vector2.one;
    
    void Awake()
    {
        playerPhysicController = GetComponent<PlayerPhysicController>();
        audioManager = new AudioManager(gameObject, SoundController.Instance.sfxAudioMixerGroup,
            SoundController.Instance.bypassAudioMixerGroup);
    }

    public void Grounded()
    {
        audioManager.PlaySound(grounded, new Vector2(0.4f,0.6f), new Vector2(0.9f, 1f), false, Vector3.down);   
    }
    
    private void Update()
    {
        float t = Mathf.Clamp01(playerPhysicController.Velocity.magnitude / 25f);
        wind.volume = Mathf.Lerp(windVolumeRange.x, windVolumeRange.y, t);
    }
}
