using System;
using UnityEngine;
using System.Collections;

public class PolygonProjectileScript : MonoBehaviour
{
    public GameObject impactParticle;
    public GameObject projectileParticle;
    public GameObject muzzleParticle;
    public GameObject[] trailParticles;

    public float speed = 10f;

    private Rigidbody rb;

    public float maxDistance = 100f;
    private float timeEnd = 0;

    public delegate void Hit(GameObject g);
    public event Hit hitEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        timeEnd = Time.time + (maxDistance / speed);
    }

    void Start()
    {
        projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
        }

        rb.velocity = transform.forward * speed;
    }

    private void Update()
    {
        if (timeEnd < Time.time)
        {
            GameObject impactP =
                Instantiate(impactParticle, transform.position, Quaternion.LookRotation(transform.up)) as GameObject;

            foreach (GameObject trail in trailParticles)
            {
                GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }

            Destroy(projectileParticle, 3f);
            Destroy(impactP, 5.0f);
            Destroy(gameObject);

            for (int i = 1; i < transform.childCount; i++)
            {
                Transform trail = transform.GetChild(i);
                trail.transform.SetParent(null);
                Destroy(trail.gameObject, 3f);
            }
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        hitEvent?.Invoke(c.gameObject);

        GameObject impactP =
            Instantiate(impactParticle, transform.position, Quaternion.LookRotation(transform.up)) as GameObject;

        foreach (GameObject trail in trailParticles)
        {
            GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
            curTrail.transform.parent = null;
            Destroy(curTrail, 3f);
        }

        Destroy(projectileParticle, 3f);
        Destroy(impactP, 5.0f);
        Destroy(gameObject);

        for (int i = 1; i < transform.childCount; i++)
        {
            Transform trail = transform.GetChild(i);
            trail.transform.SetParent(null);
            Destroy(trail.gameObject, 3f);
        }
    }
}
