using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    public MeshRenderer[] renders;
    private Color[] colors;
    public DUI_WeaponCarousel weaponCarousel;
    private int index;
    
    public void Init(int idx)
    {
        index = idx;
        colors = new Color[renders.Length];
        for (int i = 0; i < renders.Length; i++)
        {
            colors[i] = renders[i].materials[0].GetColor("_EmissionColor");
        }
    }

    public void SetEmpty(bool b)
    {
        weaponCarousel.SetAmmo(index, b);
        
        for (int i = 0; i < renders.Length; i++)
        {
            Color c = colors[i];
            
            if (b)
            {
                c.r *= 0.01f;
                c.g *= 0.01f;
                c.b *= 0.01f;
            }
            
            renders[i].materials[0].SetColor("_EmissionColor", c);
        }
    }
}
