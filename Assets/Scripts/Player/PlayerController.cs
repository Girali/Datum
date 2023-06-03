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

    private void Awake()
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

    private void Update()
    {
        PollKeys();

        playerMotor.Motor(currentRig);
        currentRig.LeftHand.Motor(currentRig.LeftControllerInputs);
        currentRig.RightHand.Motor(currentRig.RightControllerInputs);
    }
}
