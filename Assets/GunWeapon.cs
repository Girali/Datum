using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    public GameObject bullet;
    public GameObject muzzle;

    public float velocityOnShot = 10f;

    protected override void OnEnable()
    {
        base.OnEnable();
        uiAmmo.Init(this);
    }

    public override PlayerController.PlayerState Fire(ControllerInputs ci,PlayerController.PlayerState playerState)
    {
        if (ci.active.activatedThisFrame && playerState.weaponCarouselOpened == false && reloading == false && emptyAndNeedReload == false)
        {
            if (isRepulsive)
            {
                playerPhysicController.ResetGravity();
                playerPhysicController.AddVelocity(-muzzle.transform.forward * velocityOnShot);
            }

            playerState = base.Fire(ci , playerState);

            playerHandController.SetFire(recoilPower);
            weaponView.SetFire();
            Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
            
            SubAmmo();
        }
        else if(ci.active.activatedThisFrame && playerState.weaponCarouselOpened == false)
        {
            weaponView.EmptyClip();
        }

        return playerState;
    }
}
