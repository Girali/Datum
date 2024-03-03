using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerHandController playerHandController;
    public PlayerPhysicController playerPhysicController;
    public GameObject render;
    public GameObject pointer;
    public bool isRepulsive;

    public int currentAmmo = 0;
    public int totalCurrentAmmo = 0;
    public int totalAmmo = 0;
    public float reloadTime = 0;
    private float endReloadTime = 0;
    public Transform ammoPosition;
    protected bool reloading = false;

    public bool infinitAmmo = false;

    protected bool emptyAndNeedReload = false;
    
    public UI_Ammo uiAmmo;

    public void AddAmmo(float i)
    {
        totalCurrentAmmo += (int)(totalAmmo * (i * 0.5f));
    }
    
    protected void SubAmmo()
    {
        currentAmmo--;
        
        if(infinitAmmo == false)
            totalCurrentAmmo--;
        
        uiAmmo.UpdateView(this);

        if (currentAmmo == 0)
        {
            if (totalCurrentAmmo == 0 && infinitAmmo == false)
                emptyAndNeedReload = true;
            
            if(emptyAndNeedReload == false)
                StartReload();
        }
    }

    protected void StartReload()
    {
        reloading = true;
        endReloadTime = Time.unscaledTime + reloadTime;
    }

    protected void StopReload()
    {
        reloading = false;
        
        if (totalCurrentAmmo >= totalAmmo)
            currentAmmo = totalAmmo;
        else
            currentAmmo = totalCurrentAmmo;
        
        if(infinitAmmo)
            currentAmmo = totalAmmo;
    }
    
    public void ExternalUpdate()
    {
        if (reloading)
        {
            if (gameObject.activeSelf)
            {
                uiAmmo.UpdateView(1 - ((endReloadTime - Time.unscaledTime) / reloadTime));
            }
            
            if (Time.unscaledTime > endReloadTime)
            {
                StopReload();
            }
        }

        if (emptyAndNeedReload)
        {
            if (totalCurrentAmmo > 0)
            {
                emptyAndNeedReload = false;
                StartReload();
            }
        }
    }
    
    public void Init(PlayerHandController phc)
    {
        playerHandController = phc;
    }

    public void Switch(bool b)
    {
        gameObject.SetActive(b);
    }
    
    protected virtual void OnEnable()
    {
        playerHandController.Pointer = pointer.transform;
        render.SetActive(true);
    }

    private void OnDisable()
    {
        render.SetActive(false);
    }

    public virtual PlayerController.PlayerState Fire(ControllerInputs ci, PlayerController.PlayerState playerState)
    {
        return playerState;
    }
}
