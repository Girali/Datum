using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPhysicController : MonoBehaviour
{
    public float minimalForceVelocityThreshold = 1f;
    private Vector3 lastVelocity;
    private Rigidbody body = null;
    private float frameGravity;
    private int layerGround = 0;
    public bool grounded = false;
    public bool dashing = false;
    public Transform headCollider;

    public Vector3 forceVelocity;
    public float gravity = 0f;
    public Vector3 moveVelocity;
    public Vector3 dashVelocity;

    private bool useNativePhysics = false;

    public bool UseNativePhysics
    {
        set
        {
            useNativePhysics = value;
            body.useGravity = value;
        }
    }
    
    public GameObject visual;
    public float speedVisual;
    private Vector3 visualVelocity;
    private float goalHight = 1.3f;
    private float recalibaration = 0f;

    private PlayerAudioController playerAudioController;
    
    private void Awake()
    {
        playerAudioController = GetComponent<PlayerAudioController>();
        layerGround = LayerMask.GetMask("Default");
        frameGravity = Physics.gravity.y * Time.fixedDeltaTime;
        body = GetComponent<Rigidbody>();
    }

    private RaycastHit GroundedCheck()
    {
        RaycastHit hit;
        bool g = Physics.Raycast(headCollider.position, Vector3.down, out hit, 1.6f, layerGround);
        if (g == true && grounded == false)
        {
            grounded = true;
            playerAudioController.Grounded();
        }
        else if (g == false && grounded == true)
        {
            grounded = false;
        }

        return hit;
    }

    private void FixedUpdate()
    {
        RaycastHit hit = GroundedCheck();
        gravity = grounded ? 0 : gravity + frameGravity;
        if (dashing)
            gravity = 0;

        recalibaration = 0f;
        
        if (grounded)
        {
            if (hit.distance < goalHight)
            {
                recalibaration = Mathf.Lerp(8f, 0.8f, hit.distance / goalHight);
            }
        }
        
        ProcessPhysic();
        
        lastVelocity = body.velocity;

        GUI_Controller.Instance.debug1.text = "" + body.velocity.magnitude;
        GUI_Controller.Instance.debug2.text = "" + forceVelocity.magnitude;
        
        if(useNativePhysics == false)
            body.velocity = moveVelocity + new Vector3( 0, gravity, 0) + forceVelocity + new Vector3( 0, recalibaration, 0);

        if (dashing)
            body.velocity = dashVelocity;
        
        visualVelocity = Vector3.down;
        if(!grounded)
            visualVelocity = Vector3.Lerp(visualVelocity, -body.velocity + (Vector3.down *2f), speedVisual);
        
        visual.transform.rotation = Quaternion.LookRotation(visualVelocity);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        GroundedCheck();
        
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
    
    public void SetVelocity(Vector3 v)
    {
        StartCoroutine(CRT_SetVelocity(v));
    }

    IEnumerator CRT_SetVelocity(Vector3 v)
    {
        yield return null;
        
        if(useNativePhysics)
            body.velocity = v;
        else 
            forceVelocity = v;
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
