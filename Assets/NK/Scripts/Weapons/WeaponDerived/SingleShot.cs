using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Managers;
namespace Weapons
{
    //[CreateAssetMenu(menuName = "WeaponData/SingleShot")]
    [Serializable]
    public class SingleShot : WeaponData
    {
        public GameObject _projectile;
        public float _range = 10f;
        public float _projectileSpeed = 15f;
        public float _shotInterval = 0.5f;
        public float _errorDeg = 1f;
        public override void Shoot(GameObject applyingdShipObject,Ship applyingShip)
        {
            var bullet = UnityEngine.Object.Instantiate(_projectile);
            bullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
            bullet.transform.position = applyingdShipObject.transform.position;

            bullet.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,false);

            var v = applyingShip.targetObject.transform.position - applyingdShipObject.transform.position;
            bullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg + UnityEngine.Random.Range(-_errorDeg,_errorDeg));
            bullet.UpdateAsObservable()
                .Subscribe(_=>
                {
                    bullet.transform.position += _projectileSpeed * bullet.transform.right * Time.deltaTime; 
                    if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                })
                .AddTo(bullet);
        }
        public override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;

            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(_shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position,applyingShip.targetObject.transform.position) > _range)return;
                    //EventManager.Instance.PublishShoot(new EventManager.ShipShotEvent{dealingShip = applyingShip});
                    Shoot(applyingdShipObject,applyingShip);
                })
                .AddTo(applyingdShipObject);
        }
    }
}

