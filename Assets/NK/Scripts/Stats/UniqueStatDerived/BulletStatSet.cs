using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace Stats
{
    public class BulletStatSet:UniqueStatSet
    {
        [Serializable]
        public enum BulletStatType
        {
            ProjectileNum,
            BurstNum,
        }
        public Stat projectileNum = new(1);
        public Stat burstNum = new(0);
        public Stat GetStat(BulletStatType type)
        {
            return type switch
            {
                BulletStatType.BurstNum => burstNum,
                BulletStatType.ProjectileNum => projectileNum,
                _ => null
            };
        }
    }
}