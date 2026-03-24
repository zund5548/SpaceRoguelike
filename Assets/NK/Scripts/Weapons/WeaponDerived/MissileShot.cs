using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Stats;
namespace Weapons
{
    //[CreateAssetMenu(menuName = "WeaponData/MissileShot")]
    [Serializable]
    public class MissileShot: WeaponData
    {
        public GameObject projectile;
        public GameObject explosion;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public float hitTime;
        public float errorRadius;
        [Header("Unique Stat")]
        public float explosionRadius;
        public float explosionDamageMod;
        private struct MissileState
        {    
            public Vector2 targetPos;
            public Vector2 acceleration;
            public Vector2 velocity;
            public Vector2 pos;
            public float period;
        }
        public override void SetUniqueStat(Ship applyingShip)
        {
            applyingShip.uniqueStatController.AddUniqueStat(
                new MissileStatSet
                {
                    explosionRadius = new(explosionRadius),
                    explosionDamageMod = new(explosionDamageMod)
                });
        }
        private Vector2 RandomUnitVector()
        {
            float rad = UnityEngine.Random.Range(-Mathf.PI,Mathf.PI);
            return new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
        }
        private void ShootMissile(GameObject applyingdShipObject,Ship applyingShip,bool isRight)
        {
            Vector2 initTargetPos = (Vector2)applyingShip.targetObject.transform.position;
            var v = initTargetPos - (Vector2)applyingdShipObject.transform.position;
            float targetRad = Mathf.Atan2(v.y,v.x);
            float initRad = (isRight? -Mathf.PI/2f:Mathf.PI/2f) + targetRad;
            float initBurstRad = isRight? -Mathf.PI/6f:Mathf.PI/6f;
            
            Vector2 currentTargetPos = applyingShip.targetObject.transform.position;
            GameObject targetShip = applyingShip.targetObject;
            for(int i = 0;i < (int)applyingShip.projectileNum.Value;i++)
            {
                Vector2 errorOffset =  UnityEngine.Random.Range(0f,errorRadius) * RandomUnitVector();
                var missileProjectile = UnityEngine.Object.Instantiate(projectile);
                missileProjectile.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                missileProjectile.transform.position = applyingdShipObject.transform.position;
                //missileProjectile.GetComponent<Projectile>().enabled = false;
                missileProjectile.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,true,false);
                missileProjectile.UpdateAsObservable()
                    .Scan(
                        new MissileState
                        {
                            targetPos = initTargetPos + errorOffset,
                            acceleration = Vector2.zero,
                            velocity = projectileSpeed * new Vector3(Mathf.Cos(initRad + initBurstRad * i),Mathf.Sin(initRad+ initBurstRad * i),0f),
                            pos = applyingdShipObject.transform.position,
                            period = hitTime + UnityEngine.Random.Range(-0.05f,0.05f)
                        },
                        (MissileState state, UniRx.Unit _)=>
                            {
                                if(state.period > 0)
                                {
                                    //加速度計算
                                    if(applyingShip && targetShip)
                                    {
                                        currentTargetPos = targetShip.transform.position;
                                    }
                                    state.targetPos = currentTargetPos + errorOffset;
                                    state.acceleration = (state.targetPos - state.pos - state.velocity * state.period)  * 2f / (state.period * state.period);
                                    state.period -= Time.deltaTime;
                                    state.velocity += state.acceleration * Time.deltaTime;
                                    state.pos += state.velocity * Time.deltaTime;
                                }
                                return state;
                            }
                        )
                    .Subscribe(state =>
                    {
                        //missileProjectile.transform.position = state.pos;
                        missileProjectile.transform.position = Vector2.MoveTowards(missileProjectile.transform.position,state.pos,state.velocity.magnitude);
                        missileProjectile.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(state.velocity.y,state.velocity.x)*Mathf.Rad2Deg);
                        if(state.period <= 0f)
                        {
                            var p = state.targetPos;
                            UnityEngine.Object.Destroy(missileProjectile);
                            SetExplosion(applyingShip,p);
                        }
                    })
                    .AddTo(missileProjectile);
            }
        }
        public override void Shoot(GameObject applyingdShipObject,Ship applyingShip)
        {
            Vector2 initTargetPos = (Vector2)applyingShip.targetObject.transform.position;
            var v = initTargetPos - (Vector2)applyingdShipObject.transform.position;
            float targetRad = Mathf.Atan2(v.y,v.x);
            // float initRad = (isRight? -Mathf.PI/2f:Mathf.PI/2f) + targetRad;
            // float initBurstRad = isRight? -Mathf.PI/6f:Mathf.PI/6f;
            float initRad =  Mathf.Pow(-1,UnityEngine.Random.Range(0,2)) * Mathf.PI/2f;
            
            Vector2 errorOffset =  UnityEngine.Random.Range(0f,errorRadius) * RandomUnitVector();
            Vector2 currentTargetPos = applyingShip.targetObject.transform.position;
            GameObject targetShip = applyingShip.targetObject;
            var missileProjectile = UnityEngine.Object.Instantiate(projectile);
                missileProjectile.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                missileProjectile.transform.position = applyingdShipObject.transform.position;
                //missileProjectile.GetComponent<Projectile>().enabled = false;
                missileProjectile.GetComponent<Projectile>().SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,true,false);
                missileProjectile.UpdateAsObservable()
                    .Scan(
                        new MissileState
                        {
                            targetPos = initTargetPos + UnityEngine.Random.Range(0f,errorRadius) * RandomUnitVector(),
                            acceleration = Vector2.zero,
                            velocity = projectileSpeed * new Vector3(Mathf.Cos(initRad),Mathf.Sin(initRad),0f),
                            pos = applyingdShipObject.transform.position,
                            period = hitTime + UnityEngine.Random.Range(-0.05f,0.05f)
                        },
                        (MissileState state, UniRx.Unit _)=>
                            {
                                if(state.period > 0)
                                {
                                    //加速度計算
                                    if(applyingShip && targetShip)
                                    {
                                        currentTargetPos = targetShip.transform.position;
                                    }
                                    state.targetPos = currentTargetPos + errorOffset;
                                    state.acceleration = (state.targetPos - state.pos - state.velocity * state.period)  * 2f / (state.period * state.period);
                                    state.period -= Time.deltaTime;
                                    state.velocity += state.acceleration * Time.deltaTime;
                                    state.pos += state.velocity * Time.deltaTime;
                                }
                                return state;
                            }
                        )
                    .Subscribe(state =>
                    {
                        //missileProjectile.transform.position = state.pos;
                        missileProjectile.transform.position = Vector2.MoveTowards(missileProjectile.transform.position,state.pos,state.velocity.magnitude);
                        missileProjectile.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(state.velocity.y,state.velocity.x)*Mathf.Rad2Deg);
                        if(state.period <= 0f)
                        {
                            var p = state.targetPos;
                            UnityEngine.Object.Destroy(missileProjectile);
                            SetExplosion(applyingShip,p);
                        }
                    })
                    .AddTo(missileProjectile);
        }
        public override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            bool isRight = true;
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position, applyingShip.targetObject.transform.position) > range) return;
                    ShootMissile(applyingdShipObject,applyingShip,isRight);
                    isRight = !isRight;
                })
                .AddTo(applyingdShipObject);
        }
        private void SetExplosion(Ship applyingShip,Vector2 pos)
        {
            var explosionRadiusObject = UnityEngine.Object.Instantiate(explosion,pos,Quaternion.identity);
            if(applyingShip.isPlayer)explosionRadiusObject.tag = "PlayerExplosion";
            else if(!applyingShip.isPlayer)explosionRadiusObject.tag = "EnemyExplosion";
            var E = explosionRadiusObject.GetComponent<Explosion>();
            E.SetExplosion(applyingShip,(int)applyingShip.currentPower.Value,applyingShip.uniqueStatController.GetUniqueStat<MissileStatSet>().explosionRadius.Value);
        }
        
        
    }
}

