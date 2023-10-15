using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

#if !UNITY_ANDROID
using Valve.VR;

public class SteamXRInputWrapper : XRBaseController
{
    public SteamVR_Input_Sources source;
    public SteamVR_Action_Pose poseAction;

    public SteamVR_Action_Boolean selectAction;
    public SteamVR_Action_Boolean activateAction;
    public SteamVR_Action_Boolean interfaceAction;

    public SteamVR_Action_Vector2 joystick;

    void Start()
    {
        SteamVR.Initialize();
        //Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        //Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
    }

    protected override void UpdateTrackingInput(XRControllerState controllerState)
    {
        if (controllerState != null)
        {
            controllerState.position = poseAction[source].localPosition;
            controllerState.rotation = poseAction[source].localRotation;
            controllerState.inputTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;
        }
    }

    protected override void UpdateInput(XRControllerState controllerState)
    {
        if (controllerState != null)
        {
            controllerState.ResetFrameDependentStates();
            SetInputState(ref controllerState.selectInteractionState, selectAction);
            SetInputState(ref controllerState.activateInteractionState, activateAction);
            SetInputState(ref controllerState.uiPressInteractionState, interfaceAction);
        }
    }

    protected void SetInputState(ref InteractionState interactionState, SteamVR_Action_Boolean action) 
    {
        interactionState.activatedThisFrame = action.stateDown;
        interactionState.deactivatedThisFrame = action.stateUp;
        interactionState.active = action.state;
    }
}
#endif