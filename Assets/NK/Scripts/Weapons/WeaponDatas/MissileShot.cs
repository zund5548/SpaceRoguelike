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
                    applyingShip.GetNearestEnemy();
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
                        var projectile = UnityEngine.Object.Instantiate(this.projectile);
                        projectile.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                        projectile.transform.position = applyingdShipObject.transform.position;
                        projectile.GetComponent<Projectile>().power = applyingShip.power;
                        projectile.UpdateAsObservable()
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
                                    }
                                    state.period -= Time.deltaTime;
                                    state.velocity += state.acceleration * Time.deltaTime;
                                    state.pos += state.velocity * Time.deltaTime;
                                    return state;
                                }
                            )
                        .Subscribe(state =>
                        {
                            projectile.transform.position = state.pos;
                            projectile.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(state.velocity.y,state.velocity.x)*Mathf.Rad2Deg);
                            if(Vector2.Distance(projectile.transform.position,initTargetPos) <= 0.1f || state.period < 0f)
                            {
                                var expl = UnityEngine.Object.Instantiate(explosion,projectile.transform.position,Quaternion.identity);
                                expl.transform.localScale = ExplosionRadius * Vector2.one;
                                var SR = expl.GetComponent<SpriteRenderer>();
                                float remainingTime = 0.5f;
                                float t = remainingTime;
                                expl.UpdateAsObservable()
                                    .Subscribe(_ =>
                                    {
                                        t -= Time.deltaTime;
                                        Color color = new Color(SR.color.r,SR.color.g,SR.color.b,t/remainingTime);
                                        SR.color = color;
                                    })
                                    .AddTo(expl);
                                var E = expl.GetComponent<Explosion>();
                                E.power = applyingShip.power;
                                E.isPlayers = applyingShip.isPlayer;
                                UnityEngine.Object.Destroy(projectile);
                            }
                        })
                        .AddTo(projectile);
                    }
                    
                })
                .AddTo(applyingdShipObject);
        }
    }
}

