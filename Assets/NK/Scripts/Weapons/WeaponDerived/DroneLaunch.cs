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
    public class DroneLaunch : WeaponData
    {
        public GameObject droneObject;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public float angleDif;//弾と弾の間の角(deg)
        [Header("unique stat")]
        public int droneNum;
        public int droneLifetime;
        public override void SetUniqueStat(Ship applyingShip)
        {
            applyingShip.uniqueStatController.AddUniqueStat(
                new DroneStatSet
                {
                    droneNum = new(droneNum),
                    droneLifetime = new(droneLifetime),
                });
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            base.Shoot(applyingShipObject, applyingShip);
        }
        public override void ShootAction(GameObject applyingShipObject, Ship applyingShip)
        {
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    
                })
                .AddTo(applyingShipObject);
        }
    }
}

