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
        public List<Collider2D> hitCols = new List<Collider2D>();
        ContactFilter2D filter;
        void Start()
        {
            expCol = GetComponent<Collider2D>();
            filter.useTriggers = true;
            
            expCol.Overlap(filter,hitCols);
            Debug.Log(hitCols.Count);
            int n = hitCols.Count;
            for(int i = 0;i < n;i++)
            {
                if(!hitCols[i])continue;
                bool b1 = isPlayers && hitCols[i].gameObject.CompareTag("EnemyShip");
                bool b2 = !isPlayers && hitCols[i].gameObject.CompareTag("PlayerShip");
                if(b1 || b2)
                {
                    hitCols[i].gameObject.GetComponent<Ship>().DealDamage(power);
                    Debug.DrawLine(transform.position,hitCols[i].gameObject.transform.position,Color.red,1f);
                }
                
            }
        }
    }
}

