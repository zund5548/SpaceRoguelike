using System;
using UnityEngine;
namespace Stats
{
    public class MissileStatSet:UniqueStatSet
    {
        [Serializable]
        public enum MissileStatType
        {
            MissileNum,
            ExplosionRadius,
            ExplosionDamageMod
        }
        public Stat missileNum = new(1);
        public Stat explosionRadius = new(1);
        public Stat explosionDamageMod = new(0);
        public Stat GetStat(MissileStatType type)
        {
            return type switch
            {
                MissileStatType.MissileNum => missileNum,
                MissileStatType.ExplosionRadius => explosionRadius,
                MissileStatType.ExplosionDamageMod => explosionDamageMod,
                _=>null
            };
        }
    }
}

