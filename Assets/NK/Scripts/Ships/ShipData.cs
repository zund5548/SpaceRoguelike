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
            frigate,
            destroyer
        }
        
        public int maxHullPoint;
        public int maxShieldPoint;
        public int power;
        public float speed;
        [SerializeReference,SubclassSelector]
        public List<WeaponData> weaponDataList = new();
    }
}

