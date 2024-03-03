using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ammo : MonoBehaviour
{
    public Text current;
    public Image fill;
    
    public void Init(Weapon w)
    {
        if(w.infinitAmmo)
            current.text = "--";
        else
            current.text = w.totalCurrentAmmo.ToString();
        transform.position = w.ammoPosition.position;

        UpdateView(w);
    }

    public void UpdateView(Weapon w)
    {
        if(w.infinitAmmo == false)
            current.text = w.totalCurrentAmmo.ToString();

        float t = (float)w.currentAmmo / (float)w.totalAmmo;
        fill.fillAmount = t;
    }
    
    public void UpdateView(float t)
    {
        fill.fillAmount = t;
    }
}
