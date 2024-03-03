using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public float value = 10f;
    public bool ammo;

    private Transform target;
    private float startTime;
    private float duration = 1f;
    private Vector3 startPos;
    private ParticleSystem particleSystem;
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        target = GameController.Instance.playerController.collider.transform;
        startTime = Time.time;
        startPos = transform.position;
    }

    private void Update()
    {
        if (hitten == false)
        {
            float t = (Time.time - startTime) / duration;

            float dist = Vector3.Distance((target.position + Vector3.up), startPos) * 0.8f;
            
            Vector3 center = startPos + (((target.position+ Vector3.up) - startPos) * 0.5f) + (Vector3.down * dist);
            
            transform.position = EvaluateSlerpPoints(startPos, target.position + Vector3.up, center, t);

            if (t > 1f)
                PlayerHit();
        }
    }

    private bool hitten = false;
    
    private void PlayerHit()
    {
        hitten = true;
        particleSystem.Stop(true);
        particleSystem.Clear();
        Destroy(gameObject,0.5f);
        GameController.Instance.playerController.AddEnergy(value, ammo);
    }
    
    private Vector3 EvaluateSlerpPoints(Vector3 start, Vector3 end, Vector3 center, float t) {
        Vector3 startRelativeCenter = start - center;
        Vector3 endRelativeCenter = end - center;

        return Vector3.Slerp(startRelativeCenter, endRelativeCenter, t) + center;
    }
}
