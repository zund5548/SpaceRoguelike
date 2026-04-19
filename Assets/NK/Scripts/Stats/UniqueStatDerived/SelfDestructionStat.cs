using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stats;
namespace UniqueStatDerived
{
    public class SelfDestructionStat:UniqueStatSet
    {
        [Serializable]
        public enum SelfDestructionStatType
        {
            ExplosionRadius,
            ChargeTime
        }
        public Stat explosionRadius = new(5);
        public Stat chargeTime = new(0);
        public Stat GetStat(SelfDestructionStatType type)
        {
            return type switch
            {
                SelfDestructionStatType.ExplosionRadius => explosionRadius,
                SelfDestructionStatType.ChargeTime => chargeTime,
                _=>null
            };
        }
    }
}