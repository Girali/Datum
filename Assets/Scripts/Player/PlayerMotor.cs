using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private PlayerPhysicController playerPhysicController;
    private float walkSpeed = 6f * 0.2f;
    private float sprintSpeed = 6f * 0.6f;
    
    private float airControlSpeed = 8f;

    private float speed = 0;
    private bool sprinting = false;

    private float timeScale;
    private bool bulletTime;
    private float bulletTimeValue = 100f;
    private float bulletTimeSpeed = 15f;

    private bool bulletTimeUI = false;
    public UI_BulletTime bulletTimeFill;
    
    public void Init()
    {
        playerPhysicController = GetComponent<PlayerPhysicController>();
        speed = walkSpeed;
    }
    
    
    public void AddEnergy(float f)
    {
        bulletTimeValue += (7.5f * f);

        if (bulletTimeValue > 100)
            bulletTimeValue = 100;
    }

    public PlayerController.PlayerState Motor(RigInputWrapper inputs, PlayerController.PlayerState playerState)
    {
        Vector3 move = new Vector3(0, 0, 0);
        Vector3 r = inputs.Cam.transform.right;
        r.y = 0;

        Vector3 f = inputs.Cam.transform.forward;
        f.y = 0;
        
        if (inputs.RightControllerInputs.button_one.activatedThisFrame)
        {
            if (sprinting) 
            { 
                sprinting = false;
                speed = walkSpeed;
            }
            else
            {
                sprinting = true;
                speed = sprintSpeed;
            }
        }

        if (playerPhysicController.grounded == false)
        {
            speed = airControlSpeed;
        }

        float timeScaleTarget = 1f;

        if (bulletTimeFill != null)
        {
            if (inputs.RightControllerInputs.select.activatedThisFrame)
            {
                bulletTime = true;
                bulletTimeFill.SetActive(true);
            }

            if (bulletTime)
            {
                bulletTimeValue -= Time.unscaledDeltaTime * bulletTimeSpeed;
                
                timeScaleTarget = 0.1f;
                    
                if (inputs.RightControllerInputs.select.deactivatedThisFrame)
                {
                    bulletTime = false;
                }

                if (bulletTimeValue < 0)
                {
                    bulletTimeValue = 0;
                    bulletTime = false;
                }
            }
            else
            {
                bulletTimeValue += Time.unscaledDeltaTime * bulletTimeSpeed;
            }
            
            if(bulletTimeValue > 100)
            {
                bulletTimeUI = false;
                bulletTimeValue = 100;
                bulletTimeFill.SetActive(false);
            }
            else
            {
                bulletTimeFill.UpdateView(bulletTimeValue / 100f);
            }
        }

        if (playerState.weaponCarouselOpened == false)
        {
            move += inputs.RightControllerInputs.joystick.x * r.normalized;
            move += inputs.RightControllerInputs.joystick.y * f.normalized;
        }
        else
        {
            timeScaleTarget = 0.1f;
        }



        timeScale = Mathf.Lerp(timeScale, timeScaleTarget, 0.05f);

        Time.timeScale = timeScale;

        if (playerState.grapplingInUse)
        {
            playerPhysicController.AddVelocity(move * speed);
        }
        else
        {
            playerPhysicController.moveVelocity = (move * speed);
        }

        return playerState;
    }
}
