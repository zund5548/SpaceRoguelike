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
        public float projectileSpeed;
        public float shotInterval;
        public float orbitRadius;
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
            for(int i = 0;i < droneNum;i++)
            {
                SetDroneMove(i,applyingShip);
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
        public void SetDroneMove(int k,Ship applyingShip)
        {
            float deg = 360f / droneNum * k;
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
                    currentDeg += 90f * Time.deltaTime;
                    if(currentDeg >= 360f)currentDeg -= 360f;
                    var currentDronePos = orbitRadius * new Vector2(Mathf.Cos(currentDeg * Mathf.Deg2Rad),Mathf.Sin(currentDeg * Mathf.Deg2Rad)) + (Vector2)applyingShip.transform.position;
                    drone.transform.position = currentDronePos;
                })
                .AddTo(drone);
            }
    }
}

