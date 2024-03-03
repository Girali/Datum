using System.Collections;
using System.Collections.Generic;
using PolygonArsenal;
using UnityEngine;

public class PlayerBullets : MonoBehaviour
{
    private PolygonProjectileScript projectile;
    public int damage = 5;
    
    private void Awake()
    {
        projectile = GetComponent<PolygonProjectileScript>();
        projectile.hitEvent += OnHit;
    }

    void OnHit(GameObject g)
    {
        Enemy playerHitBox = g.GetComponent<Enemy>();
        if (playerHitBox != null)
        {
            playerHitBox.Hit(damage);
        }
    }
}
