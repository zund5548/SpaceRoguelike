using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
namespace Projectiles
{
    [RequireComponent(typeof(Collider2D))]
    public class Explosion : MonoBehaviour
    {
        public int power;
        public bool isPlayers;
        new Collider2D collider;
        List<Collider2D> hitShipsCols = new List<Collider2D>();
        ContactFilter2D filter;
        void Start()
        {
            collider = GetComponent<Collider2D>();
            filter.useTriggers = true;
            collider.Overlap(filter,hitShipsCols);
            foreach(var col in  hitShipsCols)
            {
                bool b1 = isPlayers && col.gameObject.CompareTag("EnemyShip");
                bool b2 = !isPlayers && col.gameObject.CompareTag("PlayerShip");
                if(!b1 && !b2)continue;
                col.gameObject.GetComponent<Ship>().DealDamage(power);
            }
        }
    }
}

