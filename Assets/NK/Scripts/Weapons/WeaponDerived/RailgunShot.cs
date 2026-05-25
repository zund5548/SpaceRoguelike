using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using Stats;
using System.Collections;
using System.Collections.Generic;
namespace Weapons
{
    [Serializable]
    public class RailgunShot : WeaponData
    {
        public GameObject railgunProjectile;
        public float range;
        [Header("unique")]
        public float projectileSpeed;
        public float projectileWidth;
        public bool enableOnKillingExplosion;
        public float onPiercingPowerScale;
        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
            new RailgunStatSet
            {
                projectileSpeed = new(projectileSpeed),
                projectileWidth  = new(projectileWidth),
                enableOnKillingExplosion = new(enableOnKillingExplosion? 1f : 0f ),
                onPiercingPowerScale = new(onPiercingPowerScale),
            });
        }
        public class RailgunBulletStatus
        {
            public List<GameObject> piercedOpponets;
            public Projectile projectile;
        }
        public  override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            SetWeaponPrefab();
            bool currentOnKillingExplosion = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().enableOnKillingExplosion.IsAble;
            if(currentOnKillingExplosion)
            {
                applyingShip.shipEventController.OnKilling
                    .Subscribe(sae =>
                    {
                        SpawnExplosion((int)applyingShip.currentPower.Value/2,4,sae.targetShip.transform.position,applyingShip);
                    })
                    .AddTo(applyingShip.gameObject);
            }
            //var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            float currentShotInterval = applyingShip.shotInterval.Value;
            applyingShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(currentShotInterval))
                .Subscribe(_ =>
                {
                    if(applyingShip.isSurged)return;
                    var targetShipObject = applyingShip.GetNearestOpponet();
                    if(!applyingShipObject || !targetShipObject)return;
                    //if(targetShipObject && targetShipObject.tag == "PlayerAnchor")return;
                    if(Vector2.Distance(applyingShipObject.transform.position,applyingShip.GetNearestOpponet().transform.position) > range)return;
                    
                    var railgunBullet = UnityEngine.Object.Instantiate(railgunProjectile);
                    railgunBullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                    railgunBullet.transform.position = applyingShipObject.transform.position;

                    var PR = railgunBullet.GetComponent<Projectile>();
                    PR.SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,true,true);

                    var v = applyingShip.GetNearestOpponet().transform.position - applyingShipObject.transform.position;
                    railgunBullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
                    var SR = railgunBullet.GetComponent<SpriteRenderer>();
                    float t = 0f;
                    float currentWidth = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().projectileWidth.Value;
                    float currentProjectileSpeed = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().projectileSpeed.Value;
                    float currentPowerScale = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().onPiercingPowerScale.Value;
                    int confirmedPower = (int)applyingShip.currentPower.Value;
                    railgunBullet.UpdateAsObservable()
                        .Scan(
                            new RailgunBulletStatus
                            {
                                piercedOpponets = new(),
                                projectile = PR
                            },
                            (RailgunBulletStatus railgunBulletStatus, UniRx.Unit _) =>
                            {
                                return railgunBulletStatus;
                            })
                            .Subscribe(railgunBulletStatus=>
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
                                    if(railgunBulletStatus.projectile._isDamaging)railgunBulletStatus.projectile._isDamaging = false;
                                    float a = SR.color.a - Time.deltaTime/2f;
                                    if(a < 0f)UnityEngine.Object.Destroy(railgunBullet);
                                    Color color  = new Color(SR.color.r,SR.color.g,SR.color.b,a);
                                    SR.color = color;
                                }
                                railgunBullet.OnTriggerEnter2DAsObservable()
                                    .Subscribe(col =>
                                    {
                                        if(railgunBulletStatus.piercedOpponets.Contains(col.gameObject))return;
                                        bool b1 = railgunBullet.CompareTag("PlayerProjectile") && col.gameObject.CompareTag("EnemyShip");
                                        bool b2 = railgunBullet.CompareTag("EnemyProjectile") && col.gameObject.CompareTag("PlayerShip");
                                        if(b1 || b2)railgunBulletStatus.piercedOpponets.Add(col.gameObject);
                                        railgunBulletStatus.projectile.SetProjectile(applyingShip,(int)(confirmedPower * (1 + railgunBulletStatus.piercedOpponets.Count * currentPowerScale / 100f)),true,true);
                                        
                                    })
                                    .AddTo(railgunBullet);                          
                            })
                            .AddTo(railgunBullet);
                    
                    
                })
                .AddTo(applyingShipObject);
        }
    }
}

