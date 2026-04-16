using System;
namespace Stats
{
    public class DroneStatSet:UniqueStatSet
    {
        [Serializable]
        public enum DroneStatType
        {
            DroneNum,
            Lifetime,
            DroneShotInterval
        }
        public Stat droneNum = new(1);
        public Stat droneLifetime = new(5);
        public Stat droneShotInterval= new(5);
        public Stat GetStat(DroneStatType type)
        {
            return type switch
            {
                DroneStatType.DroneNum => droneNum,
                DroneStatType.Lifetime => droneLifetime,
                DroneStatType.DroneShotInterval => droneShotInterval,
                _ => null
            };
        }
    }
}