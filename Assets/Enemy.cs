using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    protected int currentHealth;

    public int RPM = 10;
    protected float delay = 0;
    protected float nextFire = 0;
        
    public GameObject bullet;
    public Transform muzzle;

    protected Transform player;

    public LayerMask layerMask;
    public float maxDistance;

    public GameObject explosionPrefab;

    public GameObject[] drops;
    private bool dead = false;
    public Bounds combinedBounds = new Bounds(Vector3.zero, Vector3.zero);

    private void Awake()
    {
        bool hasBounds = false;
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (!hasBounds)
            {
                combinedBounds = r.bounds;
                hasBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(r.bounds);
            }
        }
        
        delay = (1f / RPM) * 60f;
        player = GameController.Instance.playerController.head.transform;
    }

    public float MaxBound
    {
        get
        {
            if(combinedBounds.size.x > combinedBounds.size.y && combinedBounds.size.x > combinedBounds.size.z)
                return combinedBounds.size.x * 2f;
            
            if(combinedBounds.size.y > combinedBounds.size.x && combinedBounds.size.y > combinedBounds.size.x)
                return combinedBounds.size.y * 2f;
            
            return combinedBounds.size.z * 2f;
        }
    }
    
    protected virtual void Fire()
    {
        nextFire = Time.time + delay;
        Instantiate(bullet, muzzle.position, muzzle.rotation);
    }

    public virtual void Hit(int i)
    {
        currentHealth -= i;

        if (currentHealth < 0)
        {
            Death();
        }
    }


    protected virtual void Death()
    {
        if (dead == false)
        {
            dead = true;
            Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);

            for (int i = 0; i < drops.Length; i++)
            {
                Vector2 r = Random.insideUnitCircle.normalized;
                Vector3 v = new Vector3(r.x, 0, r.y);
                Instantiate(drops[i], transform.position + v, transform.rotation);
            }
            
            Destroy(gameObject);
        }
    }
    
    protected virtual void Update()
    {
        
    }
}
