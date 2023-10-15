using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject puzzlePrefab;

    [SerializeField]
    private float spawnDelay = 1f;
    [SerializeField]
    private ParticleSystem spawnEffect;
    private BatteryItem spawned;

    public void Spawn()
    {
        StartCoroutine(CRT_Spawn());
    }

    IEnumerator CRT_Spawn()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (spawned != null && spawned.BatterySocket == null)
        {
            Destroy(spawned.gameObject);
            spawned = Instantiate(puzzlePrefab, transform.position, transform.rotation).GetComponent<BatteryItem>();
            spawnEffect.Play(true);
            SoundController.Instance.PlaySFX(SFXController.Sounds.Object_spawn, transform.position);
        }
        else if(spawned == null)
        {
            spawned = Instantiate(puzzlePrefab, transform.position, transform.rotation).GetComponent<BatteryItem>();
            spawnEffect.Play(true);
            SoundController.Instance.PlaySFX(SFXController.Sounds.Object_spawn, transform.position);
        }
    }
}
