
#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.VR;

public class SteamVRToRigWrapper : SteamXRInputWrapper
{
    [SerializeField]
    private RigInputWrapper rigInputWrapper;
    private float m_DeadzoneMin = 0.125f;
    private float m_DeadzoneMax = 0.925f;

    [SerializeField]
    private SteamVR_Action_Boolean buttonOne;
    [SerializeField]                       
    private SteamVR_Action_Boolean buttonTwo;

    private InteractionState buttonOneState;
    private InteractionState buttonTwoState;


    private Vector2 GetDeadzoneAdjustedValue(Vector2 value)
    {
        var magnitude = value.magnitude;
        var newMagnitude = GetDeadzoneAdjustedValue(magnitude);
        if (Mathf.Approximately(newMagnitude, 0f))
            value = Vector2.zero;
        else
            value *= newMagnitude / magnitude;
        return value;
    }

    private float GetDeadzoneAdjustedValue(float value)
    {
        var min = m_DeadzoneMin;
        var max = m_DeadzoneMax;

        var absValue = Mathf.Abs(value);
        if (absValue < min)
            return 0f;
        if (absValue > max)
            return Mathf.Sign(value);

        return Mathf.Sign(value) * ((absValue - min) / (max - min));
    }


    protected override void UpdateInput(XRControllerState controllerState)
    {
        base.UpdateInput(controllerState);
        
        if(controllerState != null)
        {
            buttonOneState.ResetFrameDependent();
            buttonTwoState.ResetFrameDependent();
            SetInputState(ref buttonOneState, buttonOne);
            SetInputState(ref buttonTwoState, buttonTwo);
        }
        
        Debug.LogError(controllerState.selectInteractionState.active + "   " + source);
        
        switch (source)
        {
            case Valve.VR.SteamVR_Input_Sources.LeftHand:
                rigInputWrapper.UpdateLeftHand(controllerState.selectInteractionState, controllerState.activateInteractionState, buttonOneState, buttonTwoState, GetDeadzoneAdjustedValue(joystick.axis));
                break;
            case Valve.VR.SteamVR_Input_Sources.RightHand:
                rigInputWrapper.UpdateRightHand(controllerState.selectInteractionState, controllerState.activateInteractionState, buttonOneState, buttonTwoState, GetDeadzoneAdjustedValue(joystick.axis));
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
#endif