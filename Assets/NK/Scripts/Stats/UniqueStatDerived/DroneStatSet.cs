using System;
using Items;
namespace Stats
{
    [Serializable]
    public class DroneStatSet:UniqueStatSet
    {
        [Serializable]
        public enum DroneStatType
        {
            DroneNum,
            Lifetime,
            DroneShotInterval,
             /// <summary>
            /// ドローンが消えるとき爆発
            /// </summary>
            EnableDroneExplosion,
        }
        public Stat droneNum = new(3);
        public Stat droneLifetime = new(5);
        public Stat droneShotInterval= new(5);
        public Stat enableDroneExplosion = new(0);
        public Stat GetStat(DroneStatType type)
        {
            return type switch
            {
                DroneStatType.DroneNum => droneNum,
                DroneStatType.Lifetime => droneLifetime,
                DroneStatType.DroneShotInterval => droneShotInterval,
                DroneStatType.EnableDroneExplosion => enableDroneExplosion,
                _ => null
            };
        }
        public Stat GetStat(DroneStatCollection.DroneStatType type)
        {
            return type switch
            {
                DroneStatCollection.DroneStatType.DroneNum => droneNum,
                DroneStatCollection.DroneStatType.Lifetime => droneLifetime,
                DroneStatCollection.DroneStatType.DroneShotInterval => droneShotInterval,
                _ => null
            };
        }
    }
}