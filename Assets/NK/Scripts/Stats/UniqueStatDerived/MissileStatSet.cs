using System;
using Items;
namespace Stats
{
    [Serializable]
    public class MissileStatSet:UniqueStatSet
    {
        [Serializable]
        public enum MissileStatType
        {
            MissileNum,
            MissileBurstNum,
            ExplosionRadius,
            ExplosionDamageMod
        }
        public Stat missileNum = new(1);
        public Stat missileBurstNum = new(1);
        public Stat explosionRadius = new(1);
        public Stat explosionDamageMod = new(0);
        public Stat GetStat(MissileStatType type)
        {
            return type switch
            {
                MissileStatType.MissileNum => missileNum,
                MissileStatType.MissileBurstNum => missileBurstNum,
                MissileStatType.ExplosionRadius => explosionRadius,
                MissileStatType.ExplosionDamageMod => explosionDamageMod,
                _=>null
            };
        }
    }
}

