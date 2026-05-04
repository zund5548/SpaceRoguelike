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
        }
        public Stat projectileNum = new(1);
        public Stat burstNum = new(1);
        public Stat GetStat(BulletStatType type)
        {
            return type switch
            {
                BulletStatType.BurstNum => burstNum,
                BulletStatType.ProjectileNum => projectileNum,
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