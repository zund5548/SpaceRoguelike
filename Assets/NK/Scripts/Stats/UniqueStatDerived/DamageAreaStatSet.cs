using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Items;
using UnityEngine;
namespace Stats
{
    public class DamageAreaStatSet:UniqueStatSet
    {
        [Serializable]
        public enum DamageAreaStatType
        {
            DamageInterval,
            AreaRadius,
            LastingTime
        }
        public Stat damageInterval = new(1);
        public Stat areaRadius = new(1);
        public Stat lastingTime = new(1);
        public Stat GetStat(DamageAreaStatType type)
        {
            return type switch
            {
                DamageAreaStatType.DamageInterval => damageInterval,
                DamageAreaStatType.AreaRadius => areaRadius,
                DamageAreaStatType.LastingTime => lastingTime,
                _ => null
            };
        }
    }
}