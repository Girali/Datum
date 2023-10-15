using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHack : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private List<GameObject> particles = new List<GameObject>();
    [SerializeField]
    private int particleCount = 0;
    [SerializeField]
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private Vector3 offset = Vector3.zero;

    private float maxDistance;

    private void Spawn()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            DestroyImmediate(particles[i]);
        }

        particles = new List<GameObject>();


        for (int i = 0; i < particleCount; i++)
        {
            GameObject go = Instantiate(prefab, transform);
            particles.Add(go);
            go.transform.position = prefab.transform.position + (offset * i);
        }
    }

    [Button("Preview")]
    private void Preview()
    {
        Spawn();
    }

    private void Awake()
    {
        maxDistance = Mathf.Abs(offset.y * particleCount);
        Spawn();
    }

    private void Update()
    {
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].transform.position += velocity * Time.deltaTime;
            if (Vector3.Distance(prefab.transform.position, particles[i].transform.position) > maxDistance)
            {
                particles[i].transform.position = prefab.transform.position;
            }
        }
    }
}
