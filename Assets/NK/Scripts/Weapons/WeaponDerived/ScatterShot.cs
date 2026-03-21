using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Managers;
using System.Collections;
namespace Weapons
{
    //[CreateAssetMenu( menuName = "WeaponData/ScatterShot")]
    [Serializable]
    public class ScatterShot : WeaponData
    {
        public GameObject projectile;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public int bulletNum;
        public float angleDif;//弾と弾の間の角(deg)
        public void Shoot()
        {
            
        }
        public override void Shoot(GameObject applyingdShipObject, Ship applyingShip)
        {
            float currentDeg = -angleDif * (bulletNum -1) /2f;
            
            for(int i = 0;i < bulletNum;i++)
            {
                var bullet = UnityEngine.Object.Instantiate(projectile);
                bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                bullet.transform.position = applyingdShipObject.transform.position;
                bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,false);
                var v = applyingShip.targetObject.transform.position - applyingdShipObject.transform.position;
                bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg + currentDeg);
                bullet.UpdateAsObservable()
                    .Subscribe(_=>
                    {
                        bullet.transform.position += projectileSpeed * bullet.transform.right * Time.deltaTime; 
                        if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                    })
                    .AddTo(bullet);
                currentDeg += angleDif;
            }
        }
        public override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position,applyingShip.targetObject.transform.position) > range)return;
                    //EventManager.Instance.PublishShoot(new EventManager.ShipShotEvent{dealingShip = applyingShip});
                    Shoot(applyingdShipObject,applyingShip);
                })
                .AddTo(applyingdShipObject);
        }
    }
}


