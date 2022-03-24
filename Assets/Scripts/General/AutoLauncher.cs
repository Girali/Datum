using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLauncher : MonoBehaviour
{
    public GameObject player;
    public GameObject prefab;
    public float timer = 0.2f;
    float time;

    private void Update()
    {
        transform.LookAt(player.transform);
        if(time < Time.time)
        {
            time += timer;
            Rigidbody rb = Instantiate(prefab, transform.position, transform.rotation).GetComponent<Rigidbody>();
            rb.velocity = 10f * rb.transform.forward;
            Destroy(rb.gameObject, 2f);
        }
    }
}
