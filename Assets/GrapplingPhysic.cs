using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingPhysic : MonoBehaviour
{
    private Vector3 anchor;
    private Rigidbody rb;
    private bool active = false;

    [SerializeField]
    private float maxDistance = 25f;

    private float damping = 0.99f;

    public void ChangeMaxDistance(float f)
    {
        maxDistance = f;
    }

    public float GetDistance()
    {
        return maxDistance;
    }
    
    public void SetActive(bool b, Vector3 anchorPosition)
    {
        active = b;
        anchor = anchorPosition;
    }

    public void UpdateAnchor(Vector3 anchorPosition)
    {
        anchor = anchorPosition;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(!active)
            return;

        velocity = rb.velocity;

        rb.velocity = ProcessVelocity(rb.velocity);
    }

    public Vector3 velocity;
    
    private Vector3 ProcessVelocity(Vector3 v)
    {
        Vector3 position = rb.position;
        Vector3 direction = position - anchor;
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            Vector3 n_direction = direction.normalized;
            Vector3 correctionDir = -n_direction * (distance - maxDistance);
            Vector3 side = Vector3.Cross(n_direction, Vector3.down);
            Vector3 tangent = Vector3.Cross(n_direction, side);
            
            Vector3 normal = Vector3.Cross(tangent, side);
            Vector3 newDirection = Vector3.ProjectOnPlane(v.normalized, normal);
            
            velocity = (newDirection.normalized * v.magnitude + correctionDir) * damping;
            
            return velocity;
        }

        return v;
    }
}
