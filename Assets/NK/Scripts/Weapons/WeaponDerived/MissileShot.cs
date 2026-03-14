using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Unity.Burst.Intrinsics;
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
        public int burstNum;
        public float hitTime;
        public float errorRadius;
        public float ExplosionRadius;
        private struct MissileState
        {    
            public Vector2 targetPos;
            public Vector2 acceleration;
            public Vector2 velocity;
            public Vector2 pos;
            public float period;
        }
        private Vector2 RandomUnitVector()
        {
            float rad = UnityEngine.Random.Range(-Mathf.PI,Mathf.PI);
            return new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
        }
        
        public override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            bool isRight = true;
            
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval))
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position, applyingShip.targetObject.transform.position) > range) return;
                    
                    Vector2 initTargetPos = (Vector2)applyingShip.targetObject.transform.position;
                    var v = initTargetPos - (Vector2)applyingdShipObject.transform.position;
                    float targetRad = Mathf.Atan2(v.y,v.x);
                    float initRad = (isRight? -Mathf.PI/2f:Mathf.PI/2f) + targetRad;
                    float initBurstRad = isRight? -Mathf.PI/6f:Mathf.PI/6f;
                    isRight = !isRight;
                    for(int i = 0;i < burstNum;i++)
                    {
                        var missileProjectile = UnityEngine.Object.Instantiate(projectile);
                        //missileProjectile.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                        missileProjectile.transform.position = applyingdShipObject.transform.position;
                        missileProjectile.GetComponent<Projectile>().enabled = false;
                        missileProjectile.UpdateAsObservable()
                            .Scan(
                                new MissileState
                                {
                                    targetPos = initTargetPos + errorRadius * RandomUnitVector(),
                                    acceleration = Vector2.zero,
                                    velocity = projectileSpeed * new Vector3(Mathf.Cos(initRad + initBurstRad * i),Mathf.Sin(initRad+ initBurstRad * i),0f),
                                    pos = applyingdShipObject.transform.position,
                                    period = hitTime + UnityEngine.Random.Range(-0.05f,0.05f)
                                },
                                (MissileState state, Unit _)=>
                                {
                                    if(state.period > 0)
                                    {
                                        //加速度計算
                                        state.acceleration = (state.targetPos - state.pos - state.velocity * state.period)  * 2f / (state.period * state.period);
                                        //下行をコメントアウトすると追尾しなくなる
                                        //state.targetPos = new Vector2(initTargetPos.x,initTargetPos.y);
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
                            if(state.period < 0f)
                            {
                                var p = state.pos;
                                UnityEngine.Object.Destroy(missileProjectile);
                                SetExplosion(applyingShip,p);
                            }
                        })
                        .AddTo(missileProjectile);
                    }
                })
                .AddTo(applyingdShipObject);
        }
        private void SetExplosion(Ship applyingShip,Vector2 pos)
        {
            var explosionRadiusObject = UnityEngine.Object.Instantiate(explosion,pos,Quaternion.identity);
            if(applyingShip.isPlayer)explosionRadiusObject.tag = "PlayerExplosion";
            else if(!applyingShip.isPlayer)explosionRadiusObject.tag = "EnemyExplosion";
            explosionRadiusObject.transform.localScale = ExplosionRadius * Vector2.one;
            var E = explosionRadiusObject.GetComponent<Explosion>();
            E.SetExplosion(applyingShip,(int)applyingShip.currentPower.Value);
            
            float remainingTime = 0.5f;
            float t = remainingTime;
            var SR = explosionRadiusObject.GetComponent<SpriteRenderer>();
            explosionRadiusObject.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    t -= Time.deltaTime;
                    if(t < 0f) UnityEngine.Object.Destroy(explosionRadiusObject);
                    Color color = new Color(SR.color.r,SR.color.g,SR.color.b,t/remainingTime*0.5f);
                    SR.color = color;
                })
                .AddTo(explosionRadiusObject);
        }
    }
}

