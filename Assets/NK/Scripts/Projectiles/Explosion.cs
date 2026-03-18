using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UniRx.Triggers;
using UniRx;
namespace Projectiles
{
    [RequireComponent(typeof(Collider2D))]
    public class Explosion : MonoBehaviour
    {
        public int _power{get;private set;}
        public Ship _dealtShip{get;private set;}
        float _radius;
        Collider2D expCol;
        public List<Collider2D> hitCols;
        ContactFilter2D filter = new();
        //
        private bool isDamaged = false;
        public void SetExplosion(Ship ship,int power,float radius)
        {
            _power = power;
            _dealtShip = ship;
            _radius = radius;
        }
        IEnumerator Start()
        {
            transform.localScale = _radius * Vector3.one;
            //1フレーム待つとコライダーの大きさが反映される
            yield return null;
            expCol = GetComponent<Collider2D>();
            filter.useTriggers = true;
            int n = expCol.Overlap(filter,hitCols);
            Debug.Log(transform.localScale);
            for(int i = 0;i < n;i++)
            {
                if(!hitCols[i])continue;
                bool b1 = gameObject.CompareTag("PlayerExplosion") && hitCols[i].gameObject.CompareTag("EnemyShip");
                bool b2 = gameObject.CompareTag("EnemyExplosion") && hitCols[i].gameObject.CompareTag("PlayerShip");
                if(b1 || b2)
                {
                    hitCols[i].gameObject.GetComponent<Ship>().DealDamage(_power,_dealtShip);
                    Debug.DrawLine(transform.position,hitCols[i].gameObject.transform.position,Color.red,1f);
                }
            }
            //アニメーション
            float remainingTime = 0.5f;
            float t = remainingTime;
            var SR = GetComponent<SpriteRenderer>();
            gameObject.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    t -= Time.deltaTime;
                    if(t < 0)Destroy(gameObject);
                    Color color = new Color(SR.color.r,SR.color.g,SR.color.b,t/remainingTime*0.5f);
                    SR.color = color;
                })
                .AddTo(gameObject);
            yield break;
        }
    }
}

