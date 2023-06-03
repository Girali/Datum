using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody body = null;
    private float speed = 6f * 0.2f;
    private int layerGround = 0;

    public void Init()
    {
        body = GetComponent<Rigidbody>();
        layerGround = LayerMask.GetMask("Default");
    }

    public void Motor(RigInputWrapper inputs)
    {
        float gravity = body.velocity.y;
        bool grounded = Physics.Raycast(transform.position - Vector3.up * 0.1f, Vector3.down, 0.15f, layerGround);

        gravity = grounded ? 0 : gravity;

        Vector3 move = new Vector3(0, gravity, 0);
        Vector3 r = inputs.Cam.transform.right;
        r.y = 0;

        Vector3 f = inputs.Cam.transform.forward;
        f.y = 0;

        move += inputs.RightControllerInputs.joystick.x * r.normalized;
        move += inputs.RightControllerInputs.joystick.y * f.normalized;

        body.velocity = move * speed;


        //float timeScale = Mathf.Lerp(0.2f, 1f, inputs.HeadPosTracking.TotalDistance / 0.5f);
        //Time.timeScale = timeScale;
    }
}
