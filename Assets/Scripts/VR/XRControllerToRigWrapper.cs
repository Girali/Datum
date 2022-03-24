using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class XRControllerToRigWrapper : XRController
{
    [SerializeField]
    private RigInputWrapper rigInputWrapper;
    private InputAxes m_InputBinding = InputAxes.Primary2DAxis;
    private float m_DeadzoneMin = 0.125f;
    private float m_DeadzoneMax = 0.925f;
    private enum InputAxes
    {
        Primary2DAxis = 0,
        Secondary2DAxis = 1,
    }
    private static readonly InputFeatureUsage<Vector2>[] k_Vec2UsageList =
    {
            CommonUsages.primary2DAxis,
            CommonUsages.secondary2DAxis,
    };

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
        InputFeatureUsage<Vector2> feature = k_Vec2UsageList[(int)m_InputBinding];
        Vector2 input = Vector2.zero;
        switch (controllerNode)
        {
            case XRNode.RightHand:
                {
                    if (inputDevice.TryGetFeatureValue(feature, out Vector2 controllerInput))
                    {
                        input += GetDeadzoneAdjustedValue(controllerInput);
                    }

                    rigInputWrapper.UpdateRightHand(ref controllerState.selectInteractionState, ref controllerState.activateInteractionState, ref controllerState.uiPressInteractionState, input);
                }
                break;
            case XRNode.LeftHand:
                {
                    if (inputDevice.TryGetFeatureValue(feature, out Vector2 controllerInput))
                    {
                        input += GetDeadzoneAdjustedValue(controllerInput);
                    }
                    rigInputWrapper.UpdateLeftHand(ref controllerState.selectInteractionState, ref controllerState.activateInteractionState, ref controllerState.uiPressInteractionState, input);
                }
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
            case XRNode.RightHand:
                rigInputWrapper.UpdateRightHand(controllerState.position, controllerState.rotation);
                break;
            case XRNode.LeftHand:
                rigInputWrapper.UpdateLeftHand(controllerState.position, controllerState.rotation);
                break;
            default:
                break;
        }
    }
}
