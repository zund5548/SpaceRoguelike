using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Items;
using UnityEngine;
namespace Stats
{
    [Serializable]
    public class BulletStatSet:UniqueStatSet
    {
        [Serializable]
        public enum BulletStatType
        {
            ProjectileNum,
            BurstNum,
            IsPiercing,
        }
        public float range;
        public float shotInterval;
        public float angleDif;//弾と弾の間の角(deg)
        public float maxErrorDeg;
        public float shotSpeed;
        //unique stat
        public Stat projectileNum = new(1);
        public Stat burstNum = new(1);
        public Stat isPiercing = new(0);
        public Stat GetStat(BulletStatType type)
        {
            return type switch
            {
                BulletStatType.BurstNum => burstNum,
                BulletStatType.ProjectileNum => projectileNum,
                BulletStatType.IsPiercing => isPiercing,
                _ => null
            };
        }
        public Stat GetStat(BulletStatCollection.BulletStatType type)
        {
            return type switch
            {
                BulletStatCollection.BulletStatType.BurstNum => burstNum,
                BulletStatCollection.BulletStatType.ProjectileNum => projectileNum,
                _ => null
            };
        }
    }
}