using System;
using System.Collections;
using System.Collections.Generic;
using PolygonArsenal;
using UnityEngine;

public class EnnemyBullet : MonoBehaviour
{
    private PolygonProjectileScript projectile;
    
    private void Awake()
    {
        projectile = GetComponent<PolygonProjectileScript>();
        projectile.hitEvent += OnHit;
    }

    void OnHit(GameObject g)
    {
        PlayerHitBox playerHitBox = g.GetComponent<PlayerHitBox>();
        if (playerHitBox != null)
        {
            playerHitBox.Hit();
        }
    }
}
