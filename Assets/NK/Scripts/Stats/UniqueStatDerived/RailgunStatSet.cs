using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stats
{
    [Serializable]
    public class RailgunStatSet:UniqueStatSet
    {
        [Serializable]
        public enum RailGunStatType
        {
            ProjectileSpeed,
            ProjectileWidth,
            EnableOnKillingExplosion,
        }
        public Stat projectileSpeed = new(75);
        public Stat projectileWidth = new(0.1f);
        public Stat enableOnKillingExplosion = new(0f);
        public Stat GetStat(RailGunStatType type)
        {
            return type switch
            {
                RailGunStatType.ProjectileSpeed => projectileSpeed,
                RailGunStatType.ProjectileWidth => projectileWidth,
                RailGunStatType.EnableOnKillingExplosion=> enableOnKillingExplosion,
                _ => null
            };
        }
    }
}