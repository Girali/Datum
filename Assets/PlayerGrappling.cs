using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;

public class PlayerGrappling : MonoBehaviour
{
    [SerializeField]
    private Transform grapleStart;
    private Transform grapPos;
    private Vector3 grapleTarget;
    [SerializeField]
    private LineRenderer lineRenderer;
    public MeshRenderer meshRenderer;
    
    public PlayerPhysicController playerPhysicController;
    public SwordWeapon swordWeapon;

    private SpringJoint joint;

    private Ray ray;

    public bool inUse;

    [SerializeField]
    private float maxDistance = 15f;
    private Spring spring;
    [SerializeField, BoxGroup("Anim Params")]
    private int quality;
    [SerializeField, BoxGroup("Anim Params")]
    private float damper;
    [SerializeField, BoxGroup("Anim Params")]
    private float strength;
    [SerializeField, BoxGroup("Anim Params")]
    private float velocity;
    [SerializeField, BoxGroup("Anim Params")]
    private float waveCount;
    [SerializeField, BoxGroup("Anim Params")]
    private float waveHeight;
    [SerializeField, BoxGroup("Anim Params")]
    private AnimationCurve affectCurve;
    public bool InUse { get => inUse; }

    public LayerMask layerMask;
    
    public GameObject swordVisual;
    private Vector3 hitPosition;
    private Vector3 hitDirection;
    private float hitDistance;

    private Vector3 offset;
    private Transform targetToFollow;
    
    private void Start()
    {
        spring = new Spring();
        spring.SetTarget(0);
        grapPos = new GameObject("GrapplingPosition").transform;
    }

    private void StartGrappling(RaycastHit hit)
    {
        swordWeapon.render.SetActive(false);
        swordVisual.gameObject.SetActive(true);

        hitPosition = hit.point;
        hitDirection = hitPosition - grapleStart.position;
        hitDistance = Vector3.Distance(grapleStart.position, hitPosition);

        inUse = true;
        playerPhysicController.useNativePhysics = true;
        
        grapPos.position = hit.point;
        targetToFollow = hit.collider.gameObject.transform;
        offset = hit.point - targetToFollow.position;
        grapleTarget = grapleStart.position;

        joint = playerPhysicController.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapPos.position;

        float distanceFromPoint = Vector3.Distance(transform.position, grapPos.position);

        joint.maxDistance = distanceFromPoint * 0.9f;
        joint.minDistance = joint.maxDistance * 0.9f;

        joint.spring = 1f;
        joint.damper = 0.5f;
        joint.massScale = 4.5f;

        spring.SetVelocity(velocity);
        lineRenderer.positionCount = quality + 1;
    }

    private void StopGrappling()
    {
        swordWeapon.render.SetActive(true);
        swordVisual.gameObject.SetActive(false);
        
        inUse = false;
        playerPhysicController.useNativePhysics = false;

        Vector3 v = playerPhysicController.Velocity;
        playerPhysicController.ResetVelocity();
        playerPhysicController.AddVelocity(v);

        spring.Reset();
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    public PlayerController.PlayerState Motor(ControllerInputs ci, PlayerController.PlayerState playerState)
    {
        if (swordWeapon.isRepulsive)
        {
            if (inUse)
            {
                playerState.grapplingInUse = true;
                
                if (ci.active.deactivatedThisFrame)
                {
                    StopGrappling();
                }
                
                meshRenderer.enabled = false;
            }
            else
            {
                ray = new Ray(swordWeapon.pointer.transform.position, swordWeapon.pointer.transform.forward);
                RaycastHit[] hits = Physics.SphereCastAll(ray, 0.25f, maxDistance, layerMask);
                if(hits.Length > 0)
                {
                    RaycastHit hit = hits.OrderBy((h) => h.distance).First();
                    meshRenderer.transform.position = hit.point;
                    meshRenderer.enabled = true;

                    if (ci.active.activatedThisFrame)
                    {
                        StartGrappling(hit);
                    }
                }
                else
                {
                    meshRenderer.transform.position = ray.origin + (ray.direction * maxDistance);
                    meshRenderer.enabled = false;
                }
            }
        }
        else
        {
            RaycastHit hit;
            ray = new Ray(swordWeapon.pointer.transform.position, swordWeapon.pointer.transform.forward);
            RaycastHit[] hits = Physics.SphereCastAll(ray, 0.25f, maxDistance, layerMask);
            if (hits.Length > 0)
            {
                hit = hits.OrderBy((h) => h.distance).First();
                meshRenderer.transform.position = hit.point;
                meshRenderer.enabled = true;

                if (ci.active.activatedThisFrame)
                {
                    StartAttract(hit);
                }
                else if (ci.active.deactivatedThisFrame)
                {
                    StopAttract();
                }

                if (inUse)
                {
                    if(timeEnd < Time.time)
                        StopAttract();
                }
            }
            else
            {
                meshRenderer.transform.position = ray.origin + (ray.direction * maxDistance);
                meshRenderer.enabled = false;
            }
        }

        return playerState;
    }

    private float timeEnd = 0;
    
    private void StartAttract(RaycastHit hit)
    {
        inUse = true;
        swordWeapon.render.SetActive(false);
        
        grapPos.position = hit.point;
        
        grapleTarget = grapleStart.position;

        spring.SetVelocity(velocity);
        lineRenderer.positionCount = quality + 1;
        
        Vector3 v = grapPos.position - playerPhysicController.transform.position;

        v += hit.normal * (v.magnitude * 0.3f);
        
        timeEnd = Time.time + 0.5f;
        
        playerPhysicController.ResetGravity();
        playerPhysicController.AddVelocity(v.normalized * 30f);
    }
    
    private void StopAttract()
    {
        inUse = false;
        swordWeapon.render.SetActive(true);

        spring.Reset();
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (inUse)
        {
            grapPos.position = targetToFollow.position + offset;
            grapleTarget = targetToFollow.position + offset;
            joint.connectedAnchor = grapPos.position;
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!inUse)
            return;

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = grapPos.position;
        Vector3 gunTipPosition = grapleStart.position;
        Vector3 up = grapleStart.right;

        grapleTarget = Vector3.Lerp(grapleTarget, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            Vector3 v = Vector3.Lerp(gunTipPosition, grapleTarget, delta) + offset;

            lineRenderer.SetPosition(i, v);

            if (i == lineRenderer.positionCount - 1)
            {
                float t = Vector3.Distance(v, hitPosition);
                
                swordVisual.transform.position = v + Vector3.Lerp((-hitDirection.normalized * 0.25f),Vector3.zero , t / hitDistance);
                swordVisual.transform.rotation = Quaternion.LookRotation(hitDirection);
            }
        }
    }

    public class Spring
    {
        private float strength;
        private float damper;
        private float target;
        private float velocity;
        private float value;

        public void Update(float deltaTime)
        {
            var direction = target - value >= 0 ? 1f : -1f;
            var force = Mathf.Abs(target - value) * strength;
            velocity += (force * direction - velocity * damper) * deltaTime;
            value += velocity * deltaTime;
        }

        public void Reset()
        {
            velocity = 0f;
            value = 0f;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        public void SetTarget(float target)
        {
            this.target = target;
        }

        public void SetDamper(float damper)
        {
            this.damper = damper;
        }

        public void SetStrength(float strength)
        {
            this.strength = strength;
        }

        public void SetVelocity(float velocity)
        {
            this.velocity = velocity;
        }

        public float Value => value;
    }
}