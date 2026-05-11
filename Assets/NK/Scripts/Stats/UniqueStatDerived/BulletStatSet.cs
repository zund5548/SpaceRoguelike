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
            ShotSpeed,
            MaxErrorDeg,
        }
        // public float range;
        // public float shotInterval;
        // public float angleDif;//弾と弾の間の角(deg)
        // public float maxErrorDeg;
        // public float shotSpeed;
        //unique stat
        public Stat projectileNum = new(1);
        public Stat burstNum = new(1);
        public Stat isPiercing = new(0);
        public Stat shotSpeed = new(10f);
        public Stat maxErrorDeg = new(3f);
        public Stat GetStat(BulletStatType type)
        {
            return type switch
            {
                BulletStatType.BurstNum => burstNum,
                BulletStatType.ProjectileNum => projectileNum,
                BulletStatType.IsPiercing => isPiercing,
                BulletStatType.ShotSpeed => shotSpeed,
                BulletStatType.MaxErrorDeg => maxErrorDeg,
                _ => null
            };
        }
    }
}