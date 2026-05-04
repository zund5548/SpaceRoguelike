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
        public Stat GetStat(MissileStatCollection.MissileStatType type)
        {
            return type switch
            {
                MissileStatCollection.MissileStatType.MissileNum => missileNum,
                MissileStatCollection.MissileStatType.MissileBurstNum => missileBurstNum,
                MissileStatCollection.MissileStatType.ExplosionRadius => explosionRadius,
                MissileStatCollection.MissileStatType.ExplosionDamageMod => explosionDamageMod,
                _=>null
            };
        }
    }
}

