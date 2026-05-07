using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Items;
using UnityEngine;
namespace Stats
{
    [Serializable]
    public class LaserStatSet:UniqueStatSet
    {
        [Serializable]
        public enum LaserStatType
        {
            Range,
            DamageIntervalReduction,
            LaserLastingTime,
            LaserTurnLate,
            LaserWidth
        }
        public Stat range = new(10);
        public Stat damageIntervalReduction = new(0.25f);
        public Stat laserLastingTime = new(5);
        public Stat laserTurnLate = new(90);
        public Stat laserWidth = new(0.1f);
        public Stat GetStat(LaserStatType type)
        {
            return type switch
            {
                LaserStatType.Range=> range,
                LaserStatType.DamageIntervalReduction => damageIntervalReduction,
                LaserStatType.LaserLastingTime => laserLastingTime,
                LaserStatType.LaserTurnLate => laserTurnLate,
                LaserStatType.LaserWidth => laserWidth,
                _ => null
            };
        }
    }
}