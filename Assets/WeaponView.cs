using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    public MeshRenderer[] renders;
    private Color colors;
    private Color orginalColors;
    private Color emptyColors;
    private Color targetColors;
    public DUI_WeaponCarousel weaponCarousel;
    private int index;
    [ColorUsage(true, true)]
    public Color fireColors;

    private float fireLerpSpeed = 0.2f;
    private bool lerping = false;
    private float lerpingValue = 0f;

    public AudioClip pullout;
    public AudioClip reloaded;
    public AudioClip dryShot;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = new AudioManager(gameObject, SoundController.Instance.sfxAudioMixerGroup, SoundController.Instance.bypassAudioMixerGroup);
    }

    public void Init(int idx)
    {
        index = idx;
        for (int i = 0; i < renders.Length; i++)
        {
            colors = renders[i].materials[0].GetColor("_EmissionColor");
            orginalColors = colors;
            targetColors = colors;
            Color c = colors;
            c.r *= 0.01f;
            c.g *= 0.01f;
            c.b *= 0.01f;
            emptyColors = c;
        }
    }

    public void EmptyClip()
    {
        audioManager.PlaySound(dryShot, new Vector2(0.1f, 0.2f), new Vector2(0.9f,1.1f));
    }
    
    public void Reloaded()
    {
        audioManager.PlaySound(reloaded, new Vector2(0.1f, 0.2f), new Vector2(0.9f,1f), true);
    }

    private void SetCurrentColors(Color c)
    {
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].materials[0].SetColor("_EmissionColor", c);
        }
    }

    private bool fire = false;
    
    public void SetFire()
    {
        lerpingValue = 0;
        lerping = true;
        SetCurrentColors(fireColors);
        colors = fireColors;
        targetColors = orginalColors;
    }

    public void EnableWeapon()
    {
        SetCurrentColors(emptyColors);
        colors = emptyColors;
        
        targetColors = isEmpty ? emptyColors : orginalColors;
        lerpingValue = 0;
        lerping = true;
        
        if(audioManager != null)
            audioManager.PlaySound(pullout, new Vector2(0.1f, 0.2f), new Vector2(0.9f,1f), true);
    }

    private bool isEmpty = false;
    
    public void SetEmpty(bool b)
    {
        isEmpty = b;
        weaponCarousel.SetAmmo(index, b);
        targetColors = b ? emptyColors : orginalColors;
        if (b == false)
        {
            lerpingValue = 0;
            lerping = true;
        }
    }

    private void Update()
    {
        if (lerping)
        {
            lerpingValue = Mathf.Lerp(lerpingValue, 1f, fireLerpSpeed);
            
            if (lerpingValue > 0.98f)
            {
                lerpingValue = 1f;
                lerping = false;
            }
            
            SetCurrentColors(Color.Lerp(colors, targetColors, lerpingValue));
        }
    }
}
