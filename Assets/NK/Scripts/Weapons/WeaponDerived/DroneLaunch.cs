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
        public GameObject projectile;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public float orbitRadius;
        [Header("unique stat")]
        public int droneNum;
        public int droneLifetime;
        public float droneShotInterval;
        public override void SetUniqueStat(Ship applyingShip)
        {
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
            int currentDroneNum = (int)applyingShip.uniqueStatController.GetUniqueStat<DroneStatSet>().droneNum.Value;
            for(int i = 0;i < currentDroneNum;i++)
            {
                SetDroneFeature(i,applyingShip);
            }
        }
        public override void ShootAction(GameObject applyingShipObject, Ship applyingShip)
        {
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            Debug.Log(shotInterval * (100f - trueSir)/100f);
            Observable.Timer(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
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
            
            Observable.EveryUpdate()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(currentDroneLifetime)))
                .Finally(()=>
                {
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
                    if(!applyingShip.gameObject || !applyingShip.GetNearestOpponet())return;
                    if(Vector2.Distance(applyingShip.transform.position, applyingShip.GetNearestOpponet().transform.position) > range) return;
                    if(!drone)return;
                    DroneShoot(drone.transform.position,applyingShip);
                })
                .AddTo(applyingShip.gameObject);
        }
        private void DroneShoot(Vector2 dronePos,Ship applyingShip)
        {
            var bullet = UnityEngine.Object.Instantiate(projectile);
            bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
            bullet.transform.position = dronePos;
            bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,false,true);
            var v = (Vector2)applyingShip.GetNearestOpponet().transform.position - dronePos;
            bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
            bullet.UpdateAsObservable()
                .Subscribe(_=>
                {
                    bullet.transform.position += projectileSpeed * bullet.transform.right * Time.deltaTime; 
                    if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                })
                .AddTo(bullet);
        }
    }
}

