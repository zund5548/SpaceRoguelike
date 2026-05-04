using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using System.Collections.Generic;

namespace Weapons
{
    [Serializable]
    public class LaserShot : WeaponData
    {
        public GameObject laserObject;
        public float range = 10f;
        public float shotInterval = 5f;
        public float damageInterval = 0.5f;
        public float laserLastTime = 3f;
        public float laserTurnRate = 90f;
        public override void SetUniqueStat(Ship applyingShip)
        {

        }
        private void SetLaser(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip.isSurged)return;
            var laser = UnityEngine.Object.Instantiate(laserObject);
            laser.tag = applyingShip.isPlayer?"PlayerProjectile":"EnemyProjectile";
            var v = applyingShip.GetNearestOpponet().transform.position - applyingdShipObject.transform.position;
            float initDeg = Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg;
            laser.transform.eulerAngles = new Vector3(0f,0f,initDeg);
            laser.transform.position = applyingdShipObject.transform.position;
            laser.transform.localScale = new Vector3(range,laser.transform.localScale.y,laser.transform.localScale.z);
            Vector2 targetPos = Vector2.zero;
            //コライダー
            ContactFilter2D filter = new();
            filter.useTriggers = true;
            // laser.UpdateAsObservable()
            //     .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(laserLastTime)))
            //     .Subscribe(_ =>
            //     {
            //         if(!applyingdShipObject || !applyingShip.targetObject)return;
            //         else targetPos = applyingShip.targetObject.transform.position;
            //         var w = targetPos - (Vector2)applyingdShipObject.transform.position;
            //         float targetDeg = Mathf.Atan2(w.y,w.x) * Mathf.Rad2Deg;
            //         float nextDeg = Mathf.MoveTowardsAngle(laser.transform.eulerAngles.z,targetDeg,laserTurnRate * Time.deltaTime);
            //         laser.transform.eulerAngles = new Vector3(0f,0f,nextDeg);
            //         laser.transform.position = applyingdShipObject.transform.position;
            //         float dist = Vector2.Distance(applyingdShipObject.transform.position,targetPos);
            //         //float actualDist = dist < range?dist:range;
            //         laser.transform.localScale = new Vector3(dist,laser.transform.localScale.y,laser.transform.localScale.z);
            //     },() =>
            //     {
            //         UnityEngine.Object.Destroy(laser);
            //     })
            //     .AddTo(laser);

            Observable.Timer(TimeSpan.Zero)
                .SelectMany(_ =>
                {
                    // 3秒間の処理ブロック
                    var duration = TimeSpan.FromSeconds(laserLastTime);

                    // 毎フレーム処理
                    var everyFrame = Observable.EveryUpdate()
                        .TakeUntil(Observable.Timer(duration))
                        .Do(_ =>
                        {
                            var targetObject = applyingShip.GetNearestOpponet();
                            if(!targetObject)UnityEngine.Object.Destroy(laser);
                            else targetPos = targetObject.transform.position;
                            var w = targetPos - (Vector2)applyingdShipObject.transform.position;
                            float targetDeg = Mathf.Atan2(w.y,w.x) * Mathf.Rad2Deg;
                            float nextDeg = Mathf.MoveTowardsAngle(laser.transform.eulerAngles.z,targetDeg,laserTurnRate * Time.deltaTime);
                            laser.transform.eulerAngles = new Vector3(0f,0f,nextDeg);
                            laser.transform.position = applyingdShipObject.transform.position;
                            //float dist = Vector2.Distance(applyingdShipObject.transform.position,targetPos);
                            //float actualDist = dist < range?dist:range;
                            laser.transform.localScale = new Vector3(range,laser.transform.localScale.y,laser.transform.localScale.z);
                        });
                    // 数秒ごと処理（例：1秒ごと）
                    var interval = Observable.Interval(TimeSpan.FromSeconds(damageInterval))
                        .TakeUntil(Observable.Timer(duration))
                        .Do(_ =>
                        {
                            LaserHit(laser,filter,applyingShip);
                        });
                    // 両方同時に動かす
                    return Observable.Merge(everyFrame, interval);
                })
                .Last() // 全部終わるのを待つ
                .Subscribe(_ =>
                {
                    UnityEngine.Object.Destroy(laser);
                })
                .AddTo(laser);
        }
        private void LaserHit(GameObject laserObject,ContactFilter2D filter,Ship applyingShip)
        {
            List<Collider2D> hitCols = new();
            int n = laserObject.GetComponent<Collider2D>().Overlap(filter,hitCols);
            for(int i = 0;i < n;i++)
            {   
                if(!hitCols[i])continue;
                bool b1 = laserObject.CompareTag("PlayerProjectile") && hitCols[i].gameObject.CompareTag("EnemyShip");
                bool b2 = laserObject.CompareTag("EnemyProjectile") && hitCols[i].gameObject.CompareTag("PlayerShip");
                int power = (int)applyingShip.currentPower.Value;
                bool isCrit = UnityEngine.Random.Range(1,1000) < applyingShip.critRate.Value * 10;
                int truePower =  isCrit ? power * 2 : power;
                if(b1 || b2)hitCols[i].gameObject.GetComponent<Ship>().DealDamage(truePower,isCrit,applyingShip);
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
                    var targetObject = applyingShip.GetNearestOpponet();
                    if(!targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position,targetObject.transform.position) > range)return;
                    SetLaser(applyingdShipObject,applyingShip);
                })
                .AddTo(applyingdShipObject);
        }
    }
}

