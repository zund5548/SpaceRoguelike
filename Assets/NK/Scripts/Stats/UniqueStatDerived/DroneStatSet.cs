using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace Stats
{
    public class DroneStatSet:UniqueStatSet
    {
        [Serializable]
        public enum DroneStatType
        {
            DroneNum,
            Lifetime
        }
        public Stat droneNum = new(1);
        public Stat droneLifetime = new(5);
        public Stat GetStat(DroneStatType type)
        {
            return type switch
            {
                DroneStatType.DroneNum => droneNum,
                DroneStatType.Lifetime => droneLifetime,
                _ => null
            };
        }
    }
}