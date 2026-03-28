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
            BurstNum,
        }
        public Stat burstNum = new(0);
        public Stat GetStat(BulletStatType type)
        {
            return type switch
            {
                BulletStatType.BurstNum => burstNum,
                _ => null
            };
        }
    }
}