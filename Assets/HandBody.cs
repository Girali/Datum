using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBody : MonoBehaviour
{
    private Rigidbody body;
    private Transform target;
    private Transform lerpTarget;
    public PlayerPhysicController playerPhysicController;

    public HitboxTrigger hitboxTrigger;
    
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        target = transform.parent;
        lerpTarget = new GameObject("Lerp").transform;
        transform.parent = null;
    }

    private void Update()
    {
        lerpTarget.position = Vector3.Lerp(lerpTarget.position, target.position, 0.8f);
        lerpTarget.rotation = Quaternion.Lerp(lerpTarget.rotation, target.rotation, 0.8f);
            
        if (playerPhysicController.grounded == false || playerPhysicController.Velocity.magnitude > 4f)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > 1f)
        {
            transform.position = lerpTarget.position = target.position;
            transform.rotation = lerpTarget.rotation = target.rotation;
        }
    }

    public Vector3 direction;
    public Vector3 angularDifferenceInDegrees;
    
    private void FixedUpdate()
    {
        direction = lerpTarget.position - transform.position;
        Quaternion rotationDifference = lerpTarget.rotation * Quaternion.Inverse(transform.rotation);
        angularDifferenceInDegrees = NormalizeAngles(rotationDifference.eulerAngles);
        
        if (hitboxTrigger.inside)
        {
            body.angularVelocity = angularDifferenceInDegrees;
            body.velocity = direction * (1f / Time.fixedDeltaTime);
        }
        else
        {
            if (direction.magnitude > 1f)
            {
                body.velocity = Vector3.zero;
                transform.position = lerpTarget.position;
            }
            else
                body.velocity = direction * (1f / Time.fixedDeltaTime);
            
            if (angularDifferenceInDegrees.magnitude > 50f)
            {
                body.angularVelocity = Vector3.zero;
                transform.rotation = lerpTarget.rotation;
            }
            else
                body.angularVelocity = angularDifferenceInDegrees;
        }
    }

    Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}
