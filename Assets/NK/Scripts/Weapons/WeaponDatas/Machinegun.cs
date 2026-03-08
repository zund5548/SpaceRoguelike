using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using NUnit.Framework.Constraints;
namespace Weapons
{
    [CreateAssetMenu(fileName = "Machinegun", menuName = "WeaponData/Machinegun")]
    public class Machinegun : WeaponData
    {
        public GameObject projectile;
        public int power;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public override void ShootAction(GameObject applyingdShip,Vector2 targetPos)
        {
            Debug.Log(applyingdShip);

            applyingdShip.UpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval))
                .Subscribe(_ =>
                {
                    if(!applyingdShip)return;
                    if(Vector2.Distance(applyingdShip.transform.position,targetPos) > range)return;
                    
                    var p = Instantiate(projectile);
                    p.transform.position = applyingdShip.transform.position;
                    p.UpdateAsObservable()
                        .Subscribe(_=>
                        {
                            p.transform.position += projectileSpeed * p.transform.right * Time.deltaTime; 
                            if(Vector2.Distance(p.transform.position,Vector2.zero) >= 20f)Destroy(p);
                        })
                        .AddTo(p);
                })
                .AddTo(applyingdShip);
        }
    }
}

