using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
using Weapons;
namespace Items
{
    [HideInInspector]
    [Serializable]
    public class UniqueStatCollection
    {
        public virtual UniqueStatSet GetStatSet()
        {
            return new UniqueStatSet();
        }
    }


    [Serializable]
    public class  BulletStatCollection:UniqueStatCollection
    {
        public BulletStatType bulletStatType;
        [Serializable]
        public enum BulletStatType
        {
            ProjectileNum,
            BurstNum,
        }
        public override UniqueStatSet GetStatSet()
        {
            return new BulletStatSet();
        }
    }
    [Serializable]
    public class MissileStatCollection:UniqueStatCollection
    {
        public MissileStatType missileStatType;
        [Serializable]
        public enum MissileStatType
        {
            MissileNum,
            MissileBurstNum,
            ExplosionRadius,
            ExplosionDamageMod
        }
        public override UniqueStatSet GetStatSet()
        {
            return new MissileStatSet();
        }
    }
    [Serializable]
    public class DroneStatCollection:UniqueStatCollection
    {
        public DroneStatType droneStatType;
        [Serializable]
        public enum DroneStatType
        {
            DroneNum,
            Lifetime,
            DroneShotInterval
        }
        public override UniqueStatSet GetStatSet()
        {
            return new DroneStatSet();
        }
    }
    [Serializable]
    public class SelfDestructionStatCollection:UniqueStatCollection
    {
        public SelfDestructionStatType selfDestructionStatType;
        [Serializable]
        public enum SelfDestructionStatType
        {
            ExplosionRadius,
            ChargeTime
        }
        public override UniqueStatSet GetStatSet()
        {
            return new SelfDestructionStatSet();
        }
    }
}