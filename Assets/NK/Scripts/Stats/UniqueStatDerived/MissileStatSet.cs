using System;
using UnityEngine;
namespace Stats
{
    public class MissileStatSet:UniqueStatSet
    {
        [Serializable]
        public enum MissileStatType
        {
            ExplosionRadius,
            ExplosionDamageMod
        }
        public Stat explosionRadius = new(1);
        public Stat explosionDamageMod = new(0);
        public Stat GetStat(MissileStatType type)
        {
            return type switch
            {
                MissileStatType.ExplosionRadius => explosionRadius,
                MissileStatType.ExplosionDamageMod => explosionDamageMod,
                _=>null
            };
        }
    }
}

