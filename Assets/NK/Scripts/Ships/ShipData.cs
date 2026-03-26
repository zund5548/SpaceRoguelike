using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Ships
{
    [CreateAssetMenu(menuName = "ShipData")]
    [Serializable]
    public class ShipData : ScriptableObject
    {
        public string shipName;
        public ShipType shipType;
        [Serializable]
        public enum ShipType
        {
            Frigate,
            EliteFrigate,
            Destroyer,
            EliteDestroyer
        }
        
        public int maxHullPoint;
        public int maxShieldPoint;
        public int hullResistance;
        public int shieldResistance;
        public int power;
        public int projectileNum;
        public float speed;
        [SerializeReference,SubclassSelector]
        public WeaponData weaponData;
    }
}

