using UnityEngine;
using Ships;
using UniqueStatDerived;
using Projectiles;
using UniRx;
using UniRx.Triggers;
using System;
namespace Weapons
{
    [Serializable]
    public class SelfDestruction:WeaponData
    {
        public GameObject explosion;
        public GameObject warningSymbol;
        public float reactionRange;//この範囲に敵機が入ったら起爆
        [Header("unique stat")]
        public float explosionRadius = 5f;
        public float chargeTime = 1f;
        public override void SetUniqueStat(Ship applyingShip)
        {
            applyingShip.uniqueStatController.AddUniqueStat(
                new SelfDestructionStat
                {
                    explosionRadius = new(explosionRadius),
                    chargeTime = new(chargeTime),
                });
            Debug.Log(applyingShip.uniqueStatController.GetUniqueStat<SelfDestructionStat>().explosionRadius.Value);
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            SetExplosion(applyingShip,applyingShipObject.transform.position);
        }
        public override void ShootAction(GameObject applyingShipObject, Ship applyingShip)
        {
            if(!applyingShip)return;
            applyingShipObject.UpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1f))
                .Subscribe(_ =>
                {
                    GameObject targetObject = applyingShip.GetNearestOpponet();
                    if(targetObject == null)return;
                    if(Vector2.Distance(targetObject.transform.position,applyingShipObject.transform.position) < reactionRange)Shoot(applyingShipObject,applyingShip);
                })
                .AddTo(applyingShipObject);
        }
        private void SetExplosion(Ship applyingShip,Vector2 pos)
        {
            applyingShip.isAbleToMove = false;
            var warning = UnityEngine.Object.Instantiate(warningSymbol,applyingShip.transform.position,Quaternion.identity);
            warning.transform.SetParent(applyingShip.transform);
            Observable.Timer(TimeSpan.FromSeconds(chargeTime))
                .Subscribe(_ =>
                {
                    var explosionRadiusObject = UnityEngine.Object.Instantiate(explosion,pos,Quaternion.identity);
                    if(applyingShip.isPlayer)explosionRadiusObject.tag = "PlayerExplosion";
                    else explosionRadiusObject.tag = "EnemyExplosion";
                    var ExplosionSc = explosionRadiusObject.GetComponent<Explosion>();

                    int power = (int)applyingShip.currentPower.Value;
                    float radius = applyingShip.uniqueStatController.GetUniqueStat<SelfDestructionStat>().explosionRadius.Value;
                    ExplosionSc.SetExplosion(applyingShip,power,radius);
                    applyingShip.Kill();
                })
                .AddTo(applyingShip.gameObject);
            
        }
    }
}

