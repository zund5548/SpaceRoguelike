using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
namespace Weapons
{
    [Serializable]
    public class RailgunShot : WeaponData
    {
        public GameObject projectile;
        public float range;
        public float projectileSpeed;
        public float shotInterval;
        public override void SetUniqueStat(Ship applyingShip)
        {

        }
        public  override void ShootAction(GameObject applyingdShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingdShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestOpponet();
                    if(!applyingdShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingdShipObject.transform.position,applyingShip.targetObject.transform.position) > range)return;
                    
                    var railgunBullet = UnityEngine.Object.Instantiate(projectile);
                    railgunBullet.tag = applyingShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
                    railgunBullet.transform.position = applyingdShipObject.transform.position;

                    var PR = railgunBullet.GetComponent<Projectile>();
                    PR.SetProjectile(applyingShip,(int)applyingShip.currentPower.Value,true,true);

                    var v = applyingShip.targetObject.transform.position - applyingdShipObject.transform.position;
                    railgunBullet.transform.eulerAngles = new Vector3(0f,0f,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg);
                    var SR = railgunBullet.GetComponent<SpriteRenderer>();
                    float t = 0f;
                    railgunBullet.UpdateAsObservable()
                        .Subscribe(_=>
                        {
                            // bullet.transform.position += projectileSpeed * bullet.transform.right * Time.deltaTime; 
                            // if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                            if(projectileSpeed*t < range)
                            {
                                railgunBullet.transform.localScale = new Vector2(projectileSpeed*t,0.1f);
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

