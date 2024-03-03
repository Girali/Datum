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
    
    public override PlayerController.PlayerState Fire(ControllerInputs ci,PlayerController.PlayerState playerState)
    {
        playerState = base.Fire(ci,playerState);
        
        playerState = playerGrappling.Motor(ci, playerState);
        
        return playerState;
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
    }
}
