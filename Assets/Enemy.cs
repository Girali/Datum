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
    
    private void Awake()
    {
        delay = (1f / RPM) * 60f;
        player = GameController.Instance.playerController.collider.transform;
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
        Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);

        for (int i = 0; i < drops.Length; i++)
        {
            Vector2 r = Random.insideUnitCircle.normalized;
            Vector3 v = new Vector3(r.x, 0, r.y);
            Instantiate(drops[i], transform.position + v, transform.rotation);
        }
        
        Destroy(gameObject);
    }
    
    protected virtual void Update()
    {
        
    }
}
