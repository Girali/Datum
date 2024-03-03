using System;
using UnityEngine;
using System.Collections;

namespace PolygonArsenal
{
    public class PolygonProjectileScript : MonoBehaviour
    {
        public GameObject impactParticle;
        public GameObject projectileParticle;
        public GameObject muzzleParticle;
        public GameObject[] trailParticles;
        [Header("Adjust if not using Sphere Collider")]
        public float colliderRadius = 1f;
        [Range(0f, 1f)]
        public float collideOffset = 0.15f;

        public float speed = 10f;

        public LayerMask layerMask;

        private Rigidbody rb;
        private SphereCollider sphereCollider;

        public float maxDistance = 100f;
        private float timeEnd = 0;

        public delegate void Hit(GameObject g);

        public event Hit hitEvent;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
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

        void FixedUpdate()
        {
            RaycastHit hit;

            float rad;
            if (sphereCollider)
                rad = sphereCollider.radius;
            else
                rad = colliderRadius;

            Vector3 dir = rb.velocity;
            if (rb.useGravity)
                dir += Physics.gravity * Time.deltaTime;
            dir = dir.normalized;

            float dist = rb.velocity.magnitude * Time.deltaTime;

            if (Physics.SphereCast(transform.position, rad, dir, out hit, dist, layerMask))
            {
                hitEvent?.Invoke(hit.collider.gameObject);
                transform.position = hit.point + (hit.normal * collideOffset);
            
                GameObject impactP = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;

                if (hit.transform.tag == "Destructible") // Projectile will destroy objects tagged as Destructible
                {
                    Destroy(hit.transform.gameObject);
                }

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
            else if (timeEnd < Time.time)
            {
                GameObject impactP = Instantiate(impactParticle, transform.position, transform.rotation) as GameObject;

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
    }
}