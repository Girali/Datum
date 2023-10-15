using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody body = null;
    private float walkSpeed = 6f * 0.2f;
    private float sprintSpeed = 6f * 0.6f;
    private float speed = 0;
    private int layerGround = 0;
    private bool sprinting = false;
    private float gravity = 0f;
    private float frameGravity;

    public void Init()
    {
        frameGravity = Physics.gravity.y * Time.fixedDeltaTime;
        body = GetComponent<Rigidbody>();
        layerGround = LayerMask.GetMask("Default");
        speed = walkSpeed;
    }

    private void FixedUpdate()
    {
        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.15f, layerGround);
        gravity = grounded ? 0 : gravity + frameGravity;
    }

    public void Motor(RigInputWrapper inputs)
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

        move += inputs.RightControllerInputs.joystick.x * r.normalized;
        move += inputs.RightControllerInputs.joystick.y * f.normalized;

        body.velocity = (move * speed) + new Vector3( 0, gravity, 0);

        //float timeScale = Mathf.Lerp(0.2f, 1f, inputs.HeadPosTracking.TotalDistance / 0.5f);
        //Time.timeScale = timeScale;
    }
}
