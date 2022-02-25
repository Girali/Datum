using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SteamVRToRigWrapper : SteamXRInputWrapper
{
    [SerializeField]
    private RigInputWrapper rigInputWrapper;

    protected override void UpdateInput(XRControllerState controllerState)
    {
        base.UpdateInput(controllerState);

        switch (source)
        {
            case Valve.VR.SteamVR_Input_Sources.LeftHand:
                rigInputWrapper.UpdateRightHand(ref controllerState.selectInteractionState, ref controllerState.activateInteractionState, ref controllerState.uiPressInteractionState);
                break;
            case Valve.VR.SteamVR_Input_Sources.RightHand:
                rigInputWrapper.UpdateLeftHand(ref controllerState.selectInteractionState, ref controllerState.activateInteractionState, ref controllerState.uiPressInteractionState);
                break;
            default:
                break;
        }
    }

    protected override void UpdateTrackingInput(XRControllerState controllerState)
    {
        base.UpdateTrackingInput(controllerState);

        switch (source)
        {
            case Valve.VR.SteamVR_Input_Sources.LeftHand:
                rigInputWrapper.UpdateLeftHand(controllerState.position, controllerState.rotation);
                break;
            case Valve.VR.SteamVR_Input_Sources.RightHand:
                rigInputWrapper.UpdateRightHand(controllerState.position, controllerState.rotation);
                break;
            default:
                break;
        }
    }
}
