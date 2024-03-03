using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBulletSpawner : MonoBehaviour
{
    public GameObject muzzleParticle;
    public GameObject bullet;

    public float radius = 1f;
    public float distance = 15f;

    public int bulletCount = 10;
    
    void Awake()
    {
        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
        }

        for (int i = 0; i < bulletCount; i++)
        {
            Vector2 rad = Random.insideUnitCircle;
            Vector3 dir = (transform.forward * distance) + (rad.x * radius * transform.right) +
                          (rad.y * radius * transform.up);

            Instantiate(bullet, transform.position, Quaternion.LookRotation(dir));
        }
    }
}