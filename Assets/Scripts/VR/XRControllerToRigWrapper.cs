using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRControllerToRigWrapper : XRController
{
    [SerializeField]
    private RigInputWrapper rigInputWrapper;

    protected override void UpdateInput(XRControllerState controllerState)
    {
        base.UpdateInput(controllerState);

        switch (controllerNode)
        {
            case UnityEngine.XR.XRNode.RightHand:
                rigInputWrapper.UpdateRightHand(ref controllerState.selectInteractionState, ref controllerState.activateInteractionState, ref controllerState.uiPressInteractionState);
                break;
            case UnityEngine.XR.XRNode.LeftHand:
                rigInputWrapper.UpdateLeftHand(ref controllerState.selectInteractionState, ref controllerState.activateInteractionState, ref controllerState.uiPressInteractionState);
                break;
            default:
                break;
        }
    }

    protected override void UpdateTrackingInput(XRControllerState controllerState)
    {
        base.UpdateTrackingInput(controllerState);

        switch (controllerNode)
        {
            case UnityEngine.XR.XRNode.RightHand:
                rigInputWrapper.UpdateRightHand(controllerState.position, controllerState.rotation);
                break;
            case UnityEngine.XR.XRNode.LeftHand:
                rigInputWrapper.UpdateLeftHand(controllerState.position, controllerState.rotation);
                break;
            default:
                break;
        }
    }
}
