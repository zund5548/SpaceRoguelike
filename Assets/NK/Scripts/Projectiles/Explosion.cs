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
        Collider2D expCol;
        ContactFilter2D filter;
        void Start()
        {
            expCol = GetComponent<Collider2D>();
            filter.useTriggers = true;
            var hitCols = new List<Collider2D>();
            expCol.Overlap(filter,hitCols);
            Debug.Log(hitCols.Count);
            foreach(var col in  hitCols)
            {
                bool b1 = isPlayers && col.gameObject.CompareTag("EnemyShip");
                bool b2 = !isPlayers && col.gameObject.CompareTag("PlayerShip");
                if(b1 || b2)col.gameObject.GetComponent<Ship>().DealDamage(power);
            }
        }
    }
}

