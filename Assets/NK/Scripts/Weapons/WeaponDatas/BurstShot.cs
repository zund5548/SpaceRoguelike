using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using System.Collections;
namespace Weapons
{
    [CreateAssetMenu( menuName = "WeaponData/BurstShot")]
    public class BurstShot : WeaponData
    {
        public GameObject projectile;
        public int power;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public int burstNum;
        public float burstInterval;
        public override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            // applyingdShipObject.UpdateAsObservable()
            //     .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
            //     .ThrottleFirst(TimeSpan.FromSeconds(shotInterval))
            //     .Subscribe(_ =>
            //     {
            //         Burst(applyingdShipObject,applyingShip);
            //     })
            //     .AddTo(applyingdShipObject);
            Observable.Timer(TimeSpan.FromSeconds(shotInterval))
                .SelectMany(_=>Observable.Interval(TimeSpan.FromSeconds(burstInterval)).Take(burstNum))
                .Repeat()
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestEnemy();
                    if(!applyingdShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position,applyingShip.targetObject.transform.position) > range)return;
                    
                    var bullet = Instantiate(projectile);
                    bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                    bullet.transform.position = applyingdShipObject.transform.position;

                    bullet.GetComponent<Projectile>().power = power;

                    var v = applyingShip.targetObject.transform.position - applyingdShipObject.transform.position;
                    bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
                    bullet.UpdateAsObservable()
                        .Subscribe(_=>
                        {
                            bullet.transform.position += projectileSpeed * bullet.transform.right * Time.deltaTime; 
                            if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)Destroy(bullet);
                        })
                        .AddTo(bullet);
                })
                .AddTo(applyingdShipObject);
        }
    }
}


