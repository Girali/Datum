using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicController : MonoBehaviour
{
    public float minimalForceVelocityThreshold = 1f;
    private Vector3 lastVelocity;
    private Rigidbody body = null;
    private float frameGravity;
    private int layerGround = 0;
    public bool grounded = false;
    public Transform collider;

    public Vector3 forceVelocity;
    public float gravity = 0f;
    public Vector3 moveVelocity;

    public bool useNativePhysics = false;
    
    private void Awake()
    {
        layerGround = LayerMask.GetMask("Default");
        frameGravity = Physics.gravity.y * Time.fixedDeltaTime;
        body = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        grounded = Physics.Raycast(collider.position + Vector3.up * 0.1f, Vector3.down, 0.15f, layerGround);
        gravity = grounded ? 0 : gravity + frameGravity;
        
        ProcessPhysic();
        
        lastVelocity = body.velocity;

        GUI_Controller.Instance.debug1.text = "" + body.velocity.magnitude;
        GUI_Controller.Instance.debug2.text = "" + forceVelocity.magnitude;
        GUI_Controller.Instance.debug3.text = "" + grounded;
        
        if(useNativePhysics == false)
            body.velocity = moveVelocity + new Vector3( 0, gravity, 0) + forceVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = Physics.Raycast(collider.position + Vector3.up * 0.1f, Vector3.down, 0.15f, layerGround);

        if (body.velocity.magnitude > 5)
        {
            body.velocity *= 0.75f;
            forceVelocity *= 0.1f;
        }
    }

    public void OnCollisionStay()
    {
        forceVelocity *= 0.9f;
    }

    public void ResetVelocity()
    {
        forceVelocity = Vector3.zero;
        gravity = 0;
    }

    public void ResetGravity()
    {
        gravity = 0;
    }
    
    private void ProcessPhysic()
    {
        float drag = 0.99f;

        if (grounded)
        {
            if (forceVelocity.magnitude <= 5f)
                drag = Mathf.Lerp(0.5f, 0.99f, forceVelocity.magnitude / 5f);
        }

        forceVelocity *= drag;

        if (forceVelocity.magnitude < minimalForceVelocityThreshold && forceVelocity != Vector3.zero)
        {
            forceVelocity = Vector3.zero;
        }
    }
    
    public Vector3 Velocity
    {
        get => body.velocity;
    }
    
    public void AddVelocity(Vector3 v)
    {
        StartCoroutine(CRT_AddVelocity(v));
    }

    IEnumerator CRT_AddVelocity(Vector3 v)
    {
        yield return null;
        
        if(useNativePhysics)
            body.AddForce(v);
        else 
            forceVelocity += v;
    }
}
