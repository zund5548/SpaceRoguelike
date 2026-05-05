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
        public float shotInterval;
        public float angleDif;//弾と弾の間の角(deg)
        public float maxErrorDeg;
        public float shotSpeed;
        [Header("unique stat")]
        public float burstNum;
        public int projectileNum;
        public bool isPiercing;
        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
            new BulletStatSet
            {
                projectileNum = new(projectileNum),
                burstNum = new(burstNum),
                isPiercing = new(isPiercing? 1f : 0f )
            });
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            int currentProjectileNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().projectileNum.Value;
            float currentDeg = -angleDif * (currentProjectileNum -1) /2f;
            float errorDeg = 0f;
            bool currentIsPiercing = applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().isPiercing.Value >= 1 ? true : false;
            if(maxErrorDeg != 0)errorDeg = UnityEngine.Random.Range(-maxErrorDeg,maxErrorDeg);
            for(int i = 0;i < currentProjectileNum;i++)
            {
                var bullet = UnityEngine.Object.Instantiate(projectile);
                bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                bullet.transform.position = applyingShipObject.transform.position;
                bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,currentIsPiercing,true);
                var v = applyingShip.GetNearestOpponet().transform.position - applyingShipObject.transform.position;
                bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg + (currentDeg + errorDeg));
                bullet.UpdateAsObservable()
                    .Subscribe(_=>
                    {
                        bullet.transform.position += shotSpeed * bullet.transform.right * Time.deltaTime; 
                        if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                    })
                    .AddTo(bullet);
                currentDeg += angleDif;
            } 
        }
        public override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            if(applyingShip.isSurged)return;
            ShootLoop(applyingShip).Subscribe().AddTo(applyingShipObject);
        }
        private IObservable<long> ShootLoop(Ship applyingShip)
        {
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            int currentBurstNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().burstNum.Value;
            return Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
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


