using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Stats;
namespace Weapons
{
    [Serializable]
    public class RailgunShot : WeaponData
    {
        public GameObject railgunProjectile;
        public GameObject explosion;
        public float range;
        public float shotInterval;
        [Header("unique")]
        public float projectileSpeed;
        public float projectileWidth;
        public bool enableOnHitExplosion;
        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
            new RailgunStatSet
            {
                projectileSpeed = new(projectileSpeed),
                projectileWidth  = new(projectileWidth),
                enableOnHitExplosion = new(enableOnHitExplosion? 1f : 0f )
            });
        }
        private void SetExplosion(Ship applyingShip,Vector2 pos)
        {
            var explosionRadiusObject = UnityEngine.Object.Instantiate(explosion,pos,Quaternion.identity);
            // if(applyingShip.isPlayer)explosionRadiusObject.tag = "PlayerExplosion";
            // else if(!applyingShip.isPlayer)explosionRadiusObject.tag = "EnemyExplosion";
            explosionRadiusObject.tag = applyingShip.isPlayer? "PlayerExplosion":"EnemyExplosion";
            var ExplosionSc = explosionRadiusObject.GetComponent<Explosion>();
            float radius = 3;
            ExplosionSc.SetExplosion(applyingShip,(int)applyingShip.currentPower.Value/2,radius);
        }
        public  override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            bool currentOnHitExplosion = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().projectileSpeed.Value  >= 1f? true : false;
            if(currentOnHitExplosion)
            {
                applyingShip.shipEventController.OnKilling
                    .Subscribe(sae =>
                    {
                        SetExplosion(applyingShip,sae.targetShip.transform.position);
                    })
                    .AddTo(applyingShip.gameObject);
            }
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    if(applyingShip.isSurged)return;
                    applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !applyingShip.GetNearestOpponet())return;
                    if(Vector2.Distance(applyingdShipObject.transform.position,applyingShip.GetNearestOpponet().transform.position) > range)return;
                    
                    var railgunBullet = UnityEngine.Object.Instantiate(railgunProjectile);
                    railgunBullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                    railgunBullet.transform.position = applyingdShipObject.transform.position;

                    var PR = railgunBullet.GetComponent<Projectile>();
                    PR.SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,true,true);

                    var v = applyingShip.GetNearestOpponet().transform.position - applyingdShipObject.transform.position;
                    railgunBullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
                    var SR = railgunBullet.GetComponent<SpriteRenderer>();
                    float t = 0f;
                    float currentWidth = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().projectileWidth.Value;
                    float currentProjectileSpeed = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().projectileSpeed.Value;
                    railgunBullet.UpdateAsObservable()
                        .Subscribe(_=>
                        {
                            // bullet.transform.position += projectileSpeed * bullet.transform.right * Time.deltaTime; 
                            // if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                            if(projectileSpeed * t < range)
                            {
                                railgunBullet.transform.localScale = new Vector2(projectileSpeed*t,currentWidth);
                                t += Time.deltaTime;
                            }
                            else
                            {
                                if(PR._isDamaging)PR._isDamaging = false;
                                float a = SR.color.a - Time.deltaTime/2f;
                                if(a < 0f)UnityEngine.Object.Destroy(railgunBullet);
                                Color color  = new Color(SR.color.r,SR.color.g,SR.color.b,a);
                                SR.color = color;
                            }
                        })
                        .AddTo(railgunBullet);
                })
                .AddTo(applyingdShipObject);
        }
    }
}

