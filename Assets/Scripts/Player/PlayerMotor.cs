using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    Rigidbody leftObject = null;
    Rigidbody rightObject = null;
    Transform leftTempObject = null;
    Transform rightTempObject = null;

    bool leftLerp;
    bool leftLocked = false;
    bool rightLerp;
    bool rightLocked = false;

    bool leftPortal = false;
    bool rightPortal = false;

    Rigidbody body = null;

    int raycastLayer;
    float maxDist = 5f;
    float thickness = 0.1f;
    float lerpSpeed = 10f;

    float speed = 1.5f;

    public void Init()
    {
        raycastLayer = LayerMask.GetMask("Interactable");
        body = GetComponent<Rigidbody>();
    }

    public void Motor(RigInputWrapper inputs)
    {
        if (leftObject == null)
        {
            if (inputs.LeftControllerInputs.select.activatedThisFrame)
            {
                leftPortal = true;
                inputs.HandPortal(false,true);
                //inputs.RecenterCam();

                if (leftTempObject != null)
                {
                    leftTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                    leftTempObject = null;
                    inputs.HandSelect(false, false);
                }
            }
            else if (inputs.LeftControllerInputs.select.deactivatedThisFrame)
            {
                leftPortal = false;
                inputs.HandPortal(false, false);
            }

            if (!leftPortal)
            {
                RaycastHit hit;
                if (Physics.SphereCast(inputs.LeftControllerInputs.globalPos, thickness, inputs.LeftControllerInputs.rot * Vector3.forward, out hit, maxDist, raycastLayer))
                {
                    if (leftTempObject == null)
                    {
                        leftTempObject = hit.transform;
                        leftTempObject.GetComponent<ModelOutlineController>().ShowHide(true);
                        inputs.HandSelect(false, true);
                    }
                    else if (leftTempObject != hit.transform)
                    {
                        leftTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                        leftTempObject = hit.transform;
                        leftTempObject.GetComponent<ModelOutlineController>().ShowHide(true);
                    }
                }
                else
                {
                    if (leftTempObject != null)
                    {
                        leftTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                        leftTempObject = null;
                        inputs.HandSelect(false, false);
                    }
                }

                if (leftTempObject != null)
                {
                    if (inputs.LeftControllerInputs.active.activatedThisFrame)
                    {
                        leftTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                        leftObject = leftTempObject.GetComponent<Rigidbody>();
                        leftLerp = true;
                        leftTempObject = null;
                        inputs.HandSelect(false, false);
                    }
                }
            }
        }
        else
        {
            if (leftLerp)
            {
                if ((inputs.LeftControllerInputs.globalPos - leftObject.transform.position).magnitude > 0.1f)
                {
                    float t = (inputs.LeftControllerInputs.globalPos - leftObject.transform.position).magnitude / maxDist;
                    leftObject.velocity = (inputs.LeftControllerInputs.globalPos - leftObject.transform.position) * Mathf.Lerp(lerpSpeed, 3f, t);
                    leftObject.AddTorque((leftObject.transform.rotation * Quaternion.Inverse(inputs.LeftControllerInputs.rot)).eulerAngles);
                }
                else 
                {
                    leftObject.velocity = Vector3.zero;
                    leftLocked = true;
                    leftLerp = false;
                    leftObject.isKinematic = true;
                }
            }

            if (leftLocked)
            {
                leftObject.transform.position = inputs.LeftControllerInputs.globalPos;
                leftObject.transform.rotation = inputs.LeftControllerInputs.rot;
            }

            if (inputs.LeftControllerInputs.active.deactivatedThisFrame)
            {
                leftObject.isKinematic = false;
                if (leftLerp)
                {
                    leftObject.velocity += inputs.LeftControllerInputs.deltaPosTracking.GetAVRGLimited() * 60f * 2f;
                    leftLerp = false;
                }
                else
                    leftObject.velocity = inputs.LeftControllerInputs.deltaPosTracking.GetAVRGLimited() * 60f * 2f;
                leftObject = null;
                leftLocked = false;
            }
        }

        if (rightObject == null)
        {
            if (inputs.RightControllerInputs.select.activatedThisFrame)
            {
                rightPortal = true;
                inputs.HandPortal(true, true);

                if (rightTempObject != null)
                {
                    rightTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                    rightTempObject = null;
                    inputs.HandSelect(true, false);
                }
            }
            else if (inputs.RightControllerInputs.select.deactivatedThisFrame)
            {
                rightPortal = false;
                inputs.HandPortal(true, false);
            }

            if (!rightPortal)
            {
                RaycastHit hit;
                if (Physics.SphereCast(inputs.RightControllerInputs.globalPos, thickness, inputs.RightControllerInputs.rot * Vector3.forward, out hit, maxDist, raycastLayer))
                {
                    if (rightTempObject == null)
                    {
                        rightTempObject = hit.transform;
                        rightTempObject.GetComponent<ModelOutlineController>().ShowHide(true);
                        inputs.HandSelect(true, true);
                    }
                    else if (rightTempObject != hit.transform)
                    {
                        rightTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                        rightTempObject = hit.transform;
                        rightTempObject.GetComponent<ModelOutlineController>().ShowHide(true);
                    }
                }
                else
                {
                    if (rightTempObject != null)
                    {
                        rightTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                        rightTempObject = null;
                        inputs.HandSelect(true, false);
                    }
                }

                if (rightTempObject != null)
                {
                    if (inputs.RightControllerInputs.active.activatedThisFrame)
                    {
                        rightTempObject.GetComponent<ModelOutlineController>().ShowHide(false);
                        rightObject = rightTempObject.GetComponent<Rigidbody>();
                        rightLerp = true;
                        rightTempObject = null;
                        inputs.HandSelect(true, false);
                    }
                }
            }
        }
        else
        {
            if (rightLerp)
            {
                if ((inputs.RightControllerInputs.globalPos - rightObject.transform.position).magnitude > 0.1f)
                {
                    float t = (inputs.RightControllerInputs.globalPos - rightObject.transform.position).magnitude / maxDist;
                    rightObject.velocity = (inputs.RightControllerInputs.globalPos - rightObject.transform.position) * Mathf.Lerp(lerpSpeed, 3f, t);
                    rightObject.AddTorque((rightObject.transform.rotation * Quaternion.Inverse(inputs.RightControllerInputs.rot)).eulerAngles);
                }
                else
                {
                    rightObject.velocity = Vector3.zero;
                    rightLocked = true;
                    rightLerp = false;
                    rightObject.isKinematic = true;
                }
            }

            if (rightLocked)
            {
                rightObject.transform.position = inputs.RightControllerInputs.globalPos;
                rightObject.transform.rotation = inputs.RightControllerInputs.rot;
            }

            if (inputs.RightControllerInputs.active.deactivatedThisFrame)
            {
                rightObject.isKinematic = false;
                if (rightLerp)
                {
                    rightObject.velocity += inputs.RightControllerInputs.deltaPosTracking.GetAVRGLimited() * 60f * 2f;
                    rightLerp = false;
                }
                else
                    rightObject.velocity = inputs.RightControllerInputs.deltaPosTracking.GetAVRGLimited() * 60f * 2f;
                rightObject = null;
                rightLocked = false;
            }
        }

        Vector3 move = Vector3.zero;
        Vector3 r = inputs.Cam.transform.right;
        r.y = 0;

        Vector3 f = inputs.Cam.transform.forward;
        f.y = 0;

        move += inputs.RightControllerInputs.joystick.x * r.normalized;
        move += inputs.RightControllerInputs.joystick.y * f.normalized;

        body.velocity = move * speed;

        float timeScale = Mathf.Lerp(0.2f, 1f, inputs.HeadPosTracking.TotalDistance / 0.5f);
        Time.timeScale = timeScale;
    }

}
