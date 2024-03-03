using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerMotor playerMotor;
    [SerializeField]
    RigInputWrapper steamRig;
    [SerializeField]
    RigInputWrapper xrRig;
    
    RigInputWrapper currentRig;
    bool isSteamVR;

    public GameObject collider;
    
    public void Init()
    {
        transform.parent = null;
        playerMotor = GetComponent<PlayerMotor>();

        isSteamVR = AppController.IsSteamVRActive;
        if(isSteamVR)
            currentRig = steamRig;
        else
            currentRig = xrRig;

        currentRig.Init();
        playerMotor.Init();
    }

    private void PollKeys() 
    {
        currentRig.UpdateTracking();
    }

    public void AddEnergy(float f, bool ammo)
    {
        if (ammo)
            currentRig.LeftHand.AddAmmo(f);
        else
            playerMotor.AddEnergy(f);
    }
    
    private void Update()
    {
        PollKeys();

        PlayerState playerState = new PlayerState();
        
        playerState = currentRig.LeftHand.Motor(currentRig.LeftControllerInputs, playerState);
        playerState = currentRig.RightHand.Motor(currentRig.RightControllerInputs , playerState);
        
        playerState = playerMotor.Motor(currentRig, playerState);

    }

    public class PlayerState
    {
        public bool weaponCarouselOpened = false;
        public bool grapplingInUse = false;
    }
}
