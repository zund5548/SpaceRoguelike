using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
using Weapons;
using Items;
namespace Stats
{
    [Serializable]
    public class SelfDestructionStatSet:UniqueStatSet
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
        public Stat GetStat(SelfDestructionStatCollection.SelfDestructionStatType type)
        {
            return type switch
            {
                SelfDestructionStatCollection.SelfDestructionStatType.ExplosionRadius => explosionRadius,
                SelfDestructionStatCollection.SelfDestructionStatType.ChargeTime => chargeTime,
                _=>null
            };
        }
    }
}