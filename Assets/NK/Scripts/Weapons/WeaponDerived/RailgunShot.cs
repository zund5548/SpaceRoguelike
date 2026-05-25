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
        public  override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
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
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(currentShotInterval))
                .Subscribe(_ =>
                {
                    if(applyingShip.isSurged)return;
                    var targetShipObject = applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !targetShipObject)return;
                    if(targetShipObject.tag == "PlayerAnchor")return;
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
                    float currentPowerScale = applyingShip.uniqueStatController.GetUniqueStat<RailgunStatSet>().onPiercingPowerScale.Value;
                    int confirmedPower = (int)applyingShip.currentPower.Value;
                    railgunBullet.UpdateAsObservable()
                        .Scan(new List<GameObject>(),
                        (List<GameObject> piercedOpponets, UniRx.Unit _) =>
                        {
                            return piercedOpponets;
                        })
                        .Subscribe(piercedOpponets=>
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
                            railgunBullet.OnTriggerEnter2DAsObservable()
                                .Subscribe(col =>
                                {
                                    if(piercedOpponets.Contains(col.gameObject))return;
                                    PR.SetProjectile(applyingShip,(int)(confirmedPower * (1 + piercedOpponets.Count * currentPowerScale / 100f)),true,true);
                                    piercedOpponets.Add(col.gameObject);
                                })
                                .AddTo(railgunBullet);                          
                        })
                        .AddTo(railgunBullet);
                    
                    
                })
                .AddTo(applyingdShipObject);
        }
    }
}

