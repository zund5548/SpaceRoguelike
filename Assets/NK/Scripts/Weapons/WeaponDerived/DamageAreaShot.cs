using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Managers;
using System.Collections;
using System.Collections.Generic;
using Stats;
using Unity.Collections;
namespace Weapons
{
    [Serializable]
    public class DamageAreaShot:WeaponData
    {
        public GameObject _projectile;
        public GameObject _damageAreaObject;
        public float shotInterval;
        public float radius;
        public float lastingTime;
        public override void SetUniqueStat(Ship applyingShip)
        {
            
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            ContactFilter2D filter = new();
            filter.useTriggers = true;
            var damageArea = UnityEngine.Object.Instantiate(_damageAreaObject,applyingShipObject.transform.position,Quaternion.identity);
            damageArea.transform.localScale = new Vector2(radius,radius);
            Observable.Interval(TimeSpan.FromSeconds(0.25f))
                .Subscribe(_ =>
                {
                    DamageAreaHit(damageArea,filter,applyingShip);
                })
                .AddTo(applyingShipObject);
        }
        public override void ShootAction(GameObject applyingShipObject, Ship applyingShip)
        {
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Repeat()
                .Subscribe(_ =>
                {
                    Shoot(applyingShipObject,applyingShip);
                })
                .AddTo(applyingShipObject);
        }
        private void DamageAreaHit(GameObject damageAreaObject,ContactFilter2D filter,Ship applyingShip)
        {
            List<Collider2D> hitCols = new();
            int n = damageAreaObject.GetComponent<Collider2D>().Overlap(filter,hitCols);
            for(int i = 0;i < n;i++)
            {   
                if(!hitCols[i])continue;
                bool b1 = damageAreaObject.CompareTag("PlayerProjectile") && hitCols[i].gameObject.CompareTag("EnemyShip");
                bool b2 = damageAreaObject.CompareTag("EnemyProjectile") && hitCols[i].gameObject.CompareTag("PlayerShip");
                int power = (int)applyingShip.currentPower.Value;
                bool isCrit = UnityEngine.Random.Range(1,1000) < applyingShip.critRate.Value * 10;
                int truePower =  isCrit ? power * 2 : power;
                if(b1 || b2)hitCols[i].gameObject.GetComponent<Ship>().DealDamage(truePower,isCrit,applyingShip);
            }
        }
    }
}
