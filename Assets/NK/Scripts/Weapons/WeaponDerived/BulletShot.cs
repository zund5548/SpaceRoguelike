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
        public float range;
        public float angleDif;//弾と弾の間の角(deg)
        [Header("unique stat")]
        public float burstNum;
        public int projectileNum;
        public bool isPiercing;
        public float shotSpeed;
        public float maxErrorDeg;

        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
            new BulletStatSet
            {
                projectileNum = new(projectileNum),
                burstNum = new(burstNum),
                isPiercing = new(isPiercing? 1f : 0f ),
                shotSpeed = new(shotSpeed),
                maxErrorDeg = new(maxErrorDeg)
            });
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            float currentShotSpeed = applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().shotSpeed.Value;
            float currentMaxErrorDeg = applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().maxErrorDeg.Value;
            ShootBullet(angleDif,currentMaxErrorDeg ,currentShotSpeed,applyingShipObject,applyingShip);
        }
        public override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            if(applyingShip.isSurged)return;
            SetWeaponPrefab();
            ShootLoop(applyingShip).Subscribe().AddTo(applyingShipObject);
        }
        private IObservable<long> ShootLoop(Ship applyingShip)
        {
            //var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            float currentShotInterval = applyingShip.shotInterval.Value;
            int currentBurstNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().burstNum.Value;
            //return Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
            return Observable.Timer(TimeSpan.FromSeconds(currentShotInterval))
                .SelectMany(_=>Observable.Interval(TimeSpan.FromSeconds(0.05f)).Take(currentBurstNum).Do(i =>
                {
                    var targetShip = applyingShip.GetNearestOpponet();
                    if(!applyingShip.gameObject || !targetShip)return;
                    if(Vector2.Distance(applyingShip.gameObject.transform.position, targetShip.transform.position) > range) return;
                    Shoot(applyingShip.gameObject,applyingShip);
                    applyingShip.shipEventController.PublishShoot(new ShipEventController.ShipAttackEvent{dealerShip = applyingShip});
                }).Last())
                // .Do(_ =>
                // {
                    
                // })
                .SelectMany(_=> ShootLoop(applyingShip));
        }
    }
}


