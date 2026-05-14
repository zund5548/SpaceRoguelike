using UnityEngine;
using System;
namespace Stats
{
    public class ChainStatSet:UniqueStatSet
    {
        [Serializable]
        public enum ChainStatType
        {
            ChainNum,
            ChainRange,
            EnableAddSurge,
        }
        public Stat chainNum = new(1);
        public Stat chainRange = new(1);
        public Stat enableAddSurge = new(0);
        public Stat GetStat(ChainStatType type)
        {
            return type switch
            {
                ChainStatType.ChainNum => chainNum,
                ChainStatType.ChainRange => chainRange,
                ChainStatType.EnableAddSurge => enableAddSurge,
                _ => null
            };
        }
    }
}

