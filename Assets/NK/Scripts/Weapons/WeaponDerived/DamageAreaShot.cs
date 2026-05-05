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
        //jp:プラズマ
        public GameObject _projectile;
        public GameObject _damageAreaObject;
        public float shotInterval;
        public float shotSpeed;
        [Header("unique stat")]
       
        public float damageInterval;
        public float areaRadius;
        public float lastingTime;
        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<DamageAreaStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
            new DamageAreaStatSet
            {
                damageInterval = new(damageInterval),
                areaRadius = new(areaRadius),
                lastingTime = new(lastingTime)
            });
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            if(applyingShip.isSurged)return;
            var targetShip = applyingShip.GetNearestOpponet();
            if(!applyingShip.gameObject || !targetShip)return;
            var bullet = UnityEngine.Object.Instantiate(_projectile);
            bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
            bullet.transform.position = applyingShipObject.transform.position;
            bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,false,true);
            var v = applyingShip.GetNearestOpponet().transform.position - applyingShipObject.transform.position;
            bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
            float explodeTime = 0.75f;
            bullet.UpdateAsObservable()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(explodeTime)))
                .Finally(()=>
                {
                    SetDamageArea(bullet.transform.position,applyingShipObject,applyingShip);
                    UnityEngine.Object.Destroy(bullet);
                })
                .Subscribe(_=>
                {
                    bullet.transform.position += shotSpeed * bullet.transform.right * Time.deltaTime; 
                    if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                })
                .AddTo(bullet);            
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
        private void SetDamageArea(Vector2 pos,GameObject applyingShipObject,Ship applyingShip)
        {
            ContactFilter2D filter = new();
            filter.useTriggers = true;
            var damageArea = UnityEngine.Object.Instantiate(_damageAreaObject,pos,Quaternion.identity);
            damageArea.tag = applyingShip.isPlayer? "PlayerProjectile":"EnemyProjectile";
            float currentAreaRadius = applyingShip.uniqueStatController.GetUniqueStat<DamageAreaStatSet>().areaRadius.Value;
            damageArea.transform.localScale = new Vector2(currentAreaRadius,currentAreaRadius);
            var newColor =  applyingShip.isPlayer ? Color.turquoise:Color.orange;
            float initAlpha = 150f/255f;
            newColor.a = initAlpha;
            var sr = damageArea.GetComponent<SpriteRenderer>();
            sr.color = newColor;
            IObservable<long> DealingDamageFase()
            {
                return Observable.EveryUpdate()
                    .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(lastingTime)))
                    .ThrottleFirst(TimeSpan.FromSeconds(damageInterval))
                    .Do(_=>
                    {
                         DamageAreaHit(damageArea,filter,applyingShip);
                    });
            } 
            float fadingTime = 0.5f,t = fadingTime;
            float alpha = sr.color.a;
            IObservable<long> FadingFase()
            {
                return Observable.EveryUpdate()
                    .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(fadingTime)))
                    .Finally(()=>
                    {
                        UnityEngine.Object.Destroy(damageArea);
                    })
                    .Do(_=>
                    {
                        var color = sr.color;
                        t -= Time.deltaTime;
                        color.a = t/fadingTime * initAlpha;
                        sr.color = color;
                    });
            }
            DealingDamageFase()
                .Concat(FadingFase())
                    .Subscribe()
                    .AddTo(damageArea);
        }
    }
}
