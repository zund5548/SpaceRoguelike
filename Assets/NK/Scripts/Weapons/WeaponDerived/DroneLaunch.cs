using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Managers;
using System.Collections;
using Stats;
using Unity.Collections;
namespace Weapons
{
    [Serializable]
    public class DroneLaunch : WeaponData
    {
        public GameObject droneObject;
        public float range;
        public float orbitRadius;
        public float shotSpeed;
        [Header("unique stat")]
        public int droneNum;
        public int droneLifetime;
        public float droneShotInterval;
        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
                new DroneStatSet
                {
                    droneNum = new(droneNum),
                    droneLifetime = new(droneLifetime),
                    droneShotInterval = new(droneShotInterval)
                });
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            if(applyingShip.isSurged)return;
            int currentDroneNum = (int)applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>().droneNum.Value;
            for(int i = 0;i < currentDroneNum;i++)
            {
                SetDroneFeature(i,applyingShip);
            }
        }
        public override void ShootAction(GameObject applyingShipObject, Ship applyingShip)
        {
            float currentShotInterval = applyingShip.shotInterval.Value;
            //var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            SetWeaponPrefab();
            Observable.Timer(TimeSpan.FromSeconds(currentShotInterval))
                .Repeat()
                .Subscribe(_ =>
                {
                    Shoot(applyingShipObject,applyingShip);
                })
                .AddTo(applyingShipObject);
        }
        public void SetDroneFeature(int k,Ship applyingShip)
        {
            int currentDroneNum = (int)applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>().droneNum.Value;
            float deg = 360f / currentDroneNum * k;
            float currentDeg = deg;
            var drone = UnityEngine.Object.Instantiate(droneObject);
            drone.transform.position = orbitRadius  * new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad),Mathf.Sin(deg * Mathf.Deg2Rad)) + (Vector2)applyingShip.transform.position;
            float currentDroneLifetime = (int)applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>().droneLifetime.Value;
            bool currentDroneExplosion = applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>().enableDroneExplosion.IsAble;
            Observable.EveryUpdate()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(currentDroneLifetime)))
                .Finally(()=>
                {
                    if(currentDroneExplosion)SpawnExplosion((int)applyingShip.currentPower.Value,2f,drone.transform.position,applyingShip);
                    UnityEngine.Object.Destroy(drone);
                })
                .Subscribe(_ =>
                {
                    if(!applyingShip)return;
                    currentDeg += 90f * Time.deltaTime;
                    if(currentDeg >= 360f)currentDeg -= 360f;
                    var currentDronePos = orbitRadius * new Vector2(Mathf.Cos(currentDeg * Mathf.Deg2Rad),Mathf.Sin(currentDeg * Mathf.Deg2Rad)) + (Vector2)applyingShip.transform.position;
                    drone.transform.position = currentDronePos;
                })
                .AddTo(drone);
            float currentDroneShotInterval = applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>().droneShotInterval.Value;
            Observable.Timer(TimeSpan.FromSeconds(currentDroneShotInterval))
                .Repeat()
                .Subscribe(_=>
                {
                    var targetShip = applyingShip.GetNearestOpponet();
                    if(!applyingShip.gameObject || !targetShip)return;
                    if(Vector2.Distance(applyingShip.transform.position, targetShip.transform.position) > range) return;
                    if(!drone)return;
                    DroneShoot(drone.transform.position,applyingShip);
                })
                .AddTo(applyingShip.gameObject);
            
            applyingShip.gameObject.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    UnityEngine.Object.Destroy(drone);
                });
        }
        private void DroneShoot(Vector2 dronePos,Ship applyingShip)
        {
            var bullet = UnityEngine.Object.Instantiate(_projectile);
            bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
            bullet.transform.position = dronePos;
            bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,false,true);
            var targetShip = applyingShip.GetNearestOpponet();
            if(!targetShip)return;
            var v = (Vector2)targetShip.transform.position - dronePos;
            bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
            bullet.UpdateAsObservable()
                .Subscribe(_=>
                {
                    float currentProjectileSpeed = shotSpeed;
                    bullet.transform.position += currentProjectileSpeed * bullet.transform.right * Time.deltaTime; 
                    if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                })
                .AddTo(bullet);
        }
    }
}

