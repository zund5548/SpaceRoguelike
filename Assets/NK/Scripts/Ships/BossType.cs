using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Maps;
using Managers;
using Projectiles;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
namespace Ships
{
    [Serializable]
    public abstract class BossType
    {
        public  abstract IEnumerator BossAttack(BossShip bossShip);
        public abstract void SetMove(BossShip bossShip);
    }
    [Serializable]
    public class BossA:BossType
    {
        public GameObject projectile;
        public float projectileSpeed = 5f;  
        public float shotInterval = 0.1f;
        public override IEnumerator BossAttack(BossShip bossShip)
        {
            var wfs = new WaitForSeconds(3f);
            while(true)
            {
                yield return wfs;
                for(int i = 0;i < 100;i++)
                {
                    BulletShoot(bossShip);
                    yield return new WaitForSeconds(shotInterval);
                }
            }
        }
        public override void SetMove(BossShip bossShip)
        {
            float rad = 0;
            bossShip.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    rad += Mathf.PI/24f * Time.deltaTime;
                    if(rad >= 2f*Mathf.PI)rad -= 2f*Mathf.PI;
                    bossShip.transform.position =  5f * new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
                    bossShip.transform.eulerAngles = new Vector3(0f,0,(rad+Mathf.PI/2f)*Mathf.Rad2Deg);
                })
                .AddTo(bossShip);
        }
        public  void BulletShoot(BossShip bossShip)
        {
            if(!bossShip)return;
            var targetObject = bossShip.GetNearestOpponet();
            if(!targetObject)return;
            var bullet = UnityEngine.Object.Instantiate(projectile);
            bullet.tag = bossShip.isPlayer ? "PlayerProjectile":"EnemyProjectile";
            bullet.transform.position = bossShip.transform.position;
            bullet.GetComponent<Projectile>().SetProjectile(bossShip,(int)bossShip.currentPower.Value,false,true);
            var v = targetObject.transform.position - bossShip.transform.position;
            float randRad = UnityEngine.Random.Range(-Mathf.PI/24f,Mathf.PI/24f);
            bullet.transform.eulerAngles = new Vector3(0f,0f,(Mathf.Atan2(v.y,v.x) + randRad)* Mathf.Rad2Deg);
            bullet.UpdateAsObservable()
                .Subscribe(_=>
                {
                    bullet.transform.position += projectileSpeed * bullet.transform.right * Time.deltaTime; 
                    if(Vector2.Distance(bullet.transform.position,Vector2.zero) >= 20f)UnityEngine.Object.Destroy(bullet);
                })
                .AddTo(bullet);
        }
    }
}

