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
    public Transform grapleStart;
    private Transform grapleEnd;
    public LineRenderer lineRenderer;
    public MeshRenderer sphereRenderer;
    public GameObject targetEnemy;

    public ParticleSystem dashParticles;
    public GameObject dashEndParticlesPrefab;
    
    public PlayerPhysicController playerPhysicController;
    public SwordWeapon swordWeapon;
    public GameObject swordImpactPrefab;

    private SpringJoint joint;

    private Ray ray;

    public bool inUse;
    public bool grappling;
    public bool dashing;

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

    public LayerMask hitLayerMask;
    private LayerMask layerMaskEnemies;

    public GameObject hookSwordVisual;
    private Vector3 hitDirection;
    private Vector3 hitNormal;
    private Vector3 grapleTargetLerp;
    private bool swordImpact;

    private Enemy enemyToKill;
    private Vector3 enemyPos;
    private float speedDash = 50f;
    private float dashDistance;

    private AudioManager audioManager;
    public AudioClip dashSound;
    public AudioClip grapplingSound;

    private void Awake()
    {
        layerMaskEnemies = LayerMask.NameToLayer("Ennemies");
        spring = new Spring();
        spring.SetTarget(0);
        grapleEnd = new GameObject("TargetToFollow").transform;
        audioManager = new AudioManager(gameObject, SoundController.Instance.sfxAudioMixerGroup,
            SoundController.Instance.bypassAudioMixerGroup);
    }

    private void StartVisualGrapple(RaycastHit hit)
    {
        inUse = true;

        audioManager.PlaySound(grapplingSound, new Vector2(0.2f,0.3f) , new Vector2(0.9f, 1f));

        swordWeapon.render.SetActive(false);
        hookSwordVisual.gameObject.SetActive(true);
        swordImpact = false;
        
        grapleEnd.position = hit.point;
        grapleEnd.parent = hit.collider.gameObject.transform;
        hitDirection = grapleEnd.position - transform.position;
        hitNormal = hit.normal;
        grapleTargetLerp = grapleStart.position;
        
        spring.SetVelocity(velocity);
        lineRenderer.positionCount = quality + 1;
    }

    private void StopVisual()
    {
        swordWeapon.render.SetActive(true);
        hookSwordVisual.gameObject.SetActive(false);
        inUse = false;
        grapleEnd.parent = null;
        spring.Reset();
        lineRenderer.positionCount = 0;
    }

    private void StartGrappling(RaycastHit hit)
    {
        grappling = true;
        
        playerPhysicController.UseNativePhysics = true;
        
        float distanceFromPoint = Vector3.Distance(playerPhysicController.transform.position, hit.point);

        joint = playerPhysicController.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = hit.point;
        joint.maxDistance = distanceFromPoint * 1.1f;
        joint.minDistance = joint.maxDistance * 0.05f;
        joint.spring = 800f;
        joint.damper = 0f;
        joint.massScale = 4.5f;

        StartVisualGrapple(hit);
    }

    private void StopGrappling()
    {
        grappling = false;

        playerPhysicController.UseNativePhysics = false;

        Vector3 v = playerPhysicController.Velocity;
        playerPhysicController.ResetVelocity();
        playerPhysicController.SetVelocity(v);

        Destroy(joint);
        
        StopVisual();
    }

    public PlayerController.PlayerState Motor(ControllerInputs ci, PlayerController.PlayerState playerState)
    {
        if (grappling)
        {
            playerState.grapplingInUse = true;
            
            if (ci.active.deactivatedThisFrame)
            {
                StopGrappling();
            }
            
            sphereRenderer.enabled = false;
        }
        else if(dashing)
        {
            playerState.dashing = true;

            if (dashDistance < 1f)
            {
                StopAttract();
            }
        }
        else
        {
            ray = new Ray(swordWeapon.pointer.transform.position, swordWeapon.pointer.transform.forward);
            RaycastHit[] hits = Physics.SphereCastAll(ray, 0.5f, maxDistance, hitLayerMask);
            if(hits.Length > 0)
            {
                RaycastHit hit = hits.OrderBy((h) => h.distance).First();

                if (hit.collider.gameObject.layer == layerMaskEnemies)
                {
                    Enemy enemy = hit.transform.GetComponent<Enemy>();

                    targetEnemy.transform.position = hit.transform.position;
                    targetEnemy.transform.localScale = new Vector3(enemy.MaxBound,enemy.MaxBound,enemy.MaxBound);
                    targetEnemy.SetActive(true);
                    sphereRenderer.enabled = false;
                    
                    if (ci.active.activatedThisFrame)
                    {
                        StartAttract(enemy,hit);
                    }
                }
                else
                {
                    sphereRenderer.transform.position = hit.point;
                    sphereRenderer.enabled = true;
                    targetEnemy.SetActive(false);

                    if (ci.active.activatedThisFrame)
                    {
                        StartGrappling(hit);
                    }
                }
            }
            else
            {
                sphereRenderer.transform.position = ray.origin + (ray.direction * maxDistance);
                sphereRenderer.enabled = false;
                targetEnemy.SetActive(false);
            }
        }

        return playerState;
    }


    private void StartAttract(Enemy hit, RaycastHit h)
    {
        dashing = true;
        
        audioManager.PlaySound(dashSound, new Vector2(0.2f,0.3f) , new Vector2(0.9f, 1f));

        dashParticles.Play(true);
        
        enemyToKill = hit;
        playerPhysicController.ResetGravity();
        playerPhysicController.dashing = true;

        CalculateDash();
        
        StartVisualGrapple(h);
    }

    
    void CalculateDash()
    {
        if(enemyToKill)
            enemyPos = enemyToKill.transform.position;
        
        Vector3 dir = (enemyPos - playerPhysicController.headCollider.position);
        dashDistance = dir.magnitude;
        GUI_Controller.Instance.debug3.text = "" + dashDistance;
        playerPhysicController.dashVelocity = dir.normalized * speedDash;
    }
    
    private void StopAttract()
    {
        dashing = false;

        StopVisual();

        Vector3 dir = (enemyPos - playerPhysicController.headCollider.position);
        
        dashParticles.Stop(true);
        
        Instantiate(dashEndParticlesPrefab, enemyPos, Quaternion.LookRotation(dir));
        
        playerPhysicController.ResetGravity();
        playerPhysicController.dashing = false;
        playerPhysicController.SetVelocity((enemyToKill.transform.up - playerPhysicController.Velocity.normalized).normalized * (speedDash / 3f));

        enemyToKill.Hit(50);
        enemyToKill = null;
    }

    private void Update()
    {
        if (dashing)
        {
            CalculateDash();

            Vector3 dir = (enemyPos - playerPhysicController.headCollider.position);
            dashParticles.transform.rotation = Quaternion.LookRotation(dir);
            sphereRenderer.transform.position = enemyPos;
            sphereRenderer.enabled = false;
            targetEnemy.transform.position = enemyPos;
        }
        
        if (grappling)
        {
            joint.connectedAnchor = grapleEnd.position;
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

        Vector3 grapplePoint = grapleEnd.position;
        Vector3 gunTipPosition = grapleStart.position;
        Vector3 up = grapleStart.right;

        grapleTargetLerp = Vector3.Lerp(grapleTargetLerp, grapplePoint, 0.2f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            Vector3 v = Vector3.Lerp(gunTipPosition, grapleTargetLerp, delta) + offset;

            lineRenderer.SetPosition(i, v);

            if (swordImpact == false)
            {
                if (i == lineRenderer.positionCount - 1)
                {
                    float t = Vector3.Distance(v, grapplePoint);
                    
                    if (t < 1f)
                    {
                        swordImpact = true;
                        hookSwordVisual.transform.position = grapleEnd.position + (-hitDirection.normalized * 0.25f);
                        hookSwordVisual.transform.rotation = Quaternion.LookRotation(hitDirection);
                        Instantiate(swordImpactPrefab, hookSwordVisual.transform.position, Quaternion.LookRotation(hitNormal));
                    }
                    else
                    {
                        hookSwordVisual.transform.position = v + Vector3.Lerp((-hitDirection.normalized * 0.25f),Vector3.zero , t);
                        hookSwordVisual.transform.rotation = Quaternion.LookRotation(hitDirection);
                    }
                }
            }
            else
            {
                hookSwordVisual.transform.position = grapleEnd.position + (-hitDirection.normalized * 0.25f);
                hookSwordVisual.transform.rotation = Quaternion.LookRotation(hitDirection);
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