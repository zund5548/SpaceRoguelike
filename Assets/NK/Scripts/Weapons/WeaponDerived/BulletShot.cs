using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Managers;
using System.Collections;
using Stats;
namespace Weapons
{
    //[CreateAssetMenu( menuName = "WeaponData/ScatterShot")]
    [Serializable]
    public class BulletShot : WeaponData
    {
        public GameObject projectile;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public float angleDif;//弾と弾の間の角(deg)
        [Header("unique stat")]
        public float burstNum;
        public int projectileNum;
        public override void SetUniqueStat(Ship applyingShip)
        {
            applyingShip.uniqueStatController.AddUniqueStat(
            new BulletStatSet
            {
                projectileNum = new(projectileNum),
                burstNum = new(burstNum),
            });
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            int currentProjectileNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().projectileNum.Value;
            float currentDeg = -angleDif * (currentProjectileNum -1) /2f;
            for(int i = 0;i < currentProjectileNum;i++)
            {
                var bullet = UnityEngine.Object.Instantiate(projectile);
                bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                bullet.transform.position = applyingShipObject.transform.position;
                bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,false,true);
                var v = applyingShip.GetNearestOpponet().transform.position - applyingShipObject.transform.position;
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
        public override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            // //bool isRight = true;
            // var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            // int currentBurstNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().burstNum.Value;
            // Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
            //     .SelectMany(_=>Observable.Interval(TimeSpan.FromSeconds(0.05f)).Take(currentBurstNum))
            //     .Repeat()
            //     .Subscribe(_ =>
            //     {
            //         //applyingShip.GetNearestOpponet();
            //         if(!applyingShipObject || !applyingShip.GetNearestOpponet())return;
            //         if(Vector2.Distance(applyingShipObject.transform.position, applyingShip.GetNearestOpponet().transform.position) > range) return;
            //         Shoot(applyingShipObject,applyingShip);
            //         applyingShip.shipEventController.PublishShoot(new ShipEventController.ShipShotEvent{dealingShip = applyingShip});
            //         //isRight = !isRight;
            //     })
            //     .AddTo(applyingShipObject);
            ShootLoop(applyingShip).Subscribe().AddTo(applyingShipObject);
            
        }
        private IObservable<long> ShootLoop(Ship applyingShip)
        {
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            int currentBurstNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().burstNum.Value;
            return Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .SelectMany(_=>Observable.Interval(TimeSpan.FromSeconds(0.05f)).Take(currentBurstNum).Do(i =>
                {
                    if(!applyingShip.gameObject || !applyingShip.GetNearestOpponet())return;
                    if(Vector2.Distance(applyingShip.gameObject.transform.position, applyingShip.GetNearestOpponet().transform.position) > range) return;
                    Shoot(applyingShip.gameObject,applyingShip);
                    applyingShip.shipEventController.PublishShoot(new ShipEventController.ShipShotEvent{dealingShip = applyingShip});
                }).Last())
                .Do(_ =>
                {
                    
                })
                .SelectMany(_=> ShootLoop(applyingShip));
        }
    }
}


