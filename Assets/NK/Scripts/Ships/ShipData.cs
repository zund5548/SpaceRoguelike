using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Ships
{
    [CreateAssetMenu(fileName = "ShipData", menuName = "ShipData")]
    [Serializable]
    public class ShipData : ScriptableObject
    {
        public string shipName;
        public ShipType shipType;
        [Serializable]
        public enum ShipType
        {
            frigate,
            destroyer
        }
        public int maxHullPoint;
        public int maxShieldPoint;
        public List<WeaponData> weaponDataList = new List<WeaponData>();
    }
}

