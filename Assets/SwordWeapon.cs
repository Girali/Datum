using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : Weapon
{
    public PlayerGrappling playerGrappling;
    public LineRenderer lineRenderer;
    public ParticleSystem particles;

    public HandBody handBody;
    public Transform sphere;

    public ParticleSystem hitParticles;
    public LayerMask layerMask;

    public GameObject swordCollider;
    
    public void SwordAttackMode(bool b)
    {
        if (b)
        {
            swordCollider.transform.localScale = new Vector3(2, 2, 2);
            swordCollider.transform.localPosition = new Vector3(0, 0, -1.3f);
        }
        else
        {
            swordCollider.transform.localScale = new Vector3(1, 1, 1);
            swordCollider.transform.localPosition = new Vector3(0, 0, 0);
        }
    } 
    
    public override PlayerController.PlayerState Fire(ControllerInputs ci,PlayerController.PlayerState playerState)
    {
        playerState = base.Fire(ci,playerState);
        
        playerState = playerGrappling.Motor(ci, playerState);
        
        return playerState;
    }

    private int countColliderEntered = 0;
    private Enemy enemy;

    private bool hitting;
    
    public void OnEnter(Collider other)
    {
        countColliderEntered++;

        if (hitting == false)
        {
            hitting = countColliderEntered > 0;
            hitParticles.Play(true);
        }
    }

    public void OnExit(Collider other)
    {
        countColliderEntered--;

        if (hitting == true)
        {
            hitting = countColliderEntered > 0;
            hitParticles.Stop(true);
            enemy = other.GetComponent<Enemy>();
        }
    }

    private void Update()
    {
        if (!playerGrappling.inUse)
        {
            if (handBody.direction.magnitude > 0.04f)
            {
                if(!particles.isPlaying)
                    particles.Play();
            }
            else
            {
                if(particles.isPlaying)
                    particles.Stop();
            }

            if (lineRenderer.enabled == false)
            {
                lineRenderer.enabled = true;
            }
            
            lineRenderer.SetPosition(0, pointer.transform.position);
            lineRenderer.SetPosition(1, sphere.position);
        }
        else
        {
            if(particles.isPlaying)
                particles.Stop();
            
            if (lineRenderer.enabled == true)
                lineRenderer.enabled = false;
        }

        if (hitting)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 1.5f, layerMask))
            {
                hitParticles.transform.position = hit.point;
                hitParticles.transform.rotation = Quaternion.LookRotation(hit.normal);
            }

            if (enemy)
            {
                enemy.Hit(50);
            }
        }
    }
}
