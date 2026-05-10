using UnityEngine;
using Ships;
using System;
using Projectiles;
using Stats;
using UniRx;
using UniRx.Triggers;
namespace Weapons
{
    //SubclassSelectorで各々のshipDataに設定
    [Serializable]
    public class WeaponData
    {
        public string weaponName;
        // public GameObject _projectile;
        // public GameObject _explosion;
        [HideInInspector]
        public GameObject _projectile;
        [HideInInspector]
        public GameObject _explosion;
        /// <summary>射撃間隔はこの数値(percent)以上早くならない</summary>
        public const float MAX_ShotIntervalReduction = 90f;
        public virtual void ShootAction(GameObject applyingShipObject,Ship applyingShip){}
        public virtual void Shoot(GameObject applyingShipObject,Ship applyingShip){}
        public virtual void SetUniqueStat(Ship applyingShip){}
        
        public void SetWeaponPrefab()
        {
            _projectile = (GameObject)Resources.Load("Projectile");
            _explosion = (GameObject)Resources.Load("Explosion");
        }
        public void ShootBullet(float angleDif,float maxErrorDeg,float shotSpeed,GameObject applyingShipObject, Ship applyingShip)
        {
            int currentProjectileNum = (int)applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().projectileNum.Value;
            float currentDeg = - angleDif * (currentProjectileNum -1) /2f;
            float errorDeg = 0f;
            bool cip;
            if(applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>() == null)cip = false;
            else cip = applyingShip.uniqueStatController.GetUniqueStat<BulletStatSet>().isPiercing.IsAble;
            bool currentIsPiercing = cip;
            if(maxErrorDeg != 0)errorDeg = UnityEngine.Random.Range(-maxErrorDeg,maxErrorDeg);
            
            for(int i = 0;i < currentProjectileNum;i++)
            {
                var bullet = UnityEngine.Object.Instantiate(_projectile);
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

        public void SpawnExplosion(int power,float radius,Vector2 pos,Ship applyingShip)
        {
            var explosionRadiusObject = UnityEngine.Object.Instantiate(_explosion,pos,Quaternion.identity);
            // if(applyingShip.isPlayer)explosionRadiusObject.tag = "PlayerExplosion";
            // else if(!applyingShip.isPlayer)explosionRadiusObject.tag = "EnemyExplosion";
            explosionRadiusObject.tag = applyingShip.isPlayer? "PlayerExplosion":"EnemyExplosion";
            var ExplosionSc = explosionRadiusObject.GetComponent<Explosion>();
            ExplosionSc.SetExplosion(applyingShip,power,radius);
        }
    }
}

