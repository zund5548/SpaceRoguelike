using Ships;
using UnityEngine;
using Managers;
using Stats;
using System;
using System.Collections.Generic;

namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ShipSpeedEffect")]
    [Serializable]
    public class ShipSpeedEffect:ItemEffect
    {
        public float value;
        public ModType modType;
        public override void OnApply()
        {
            if(isPlayers)ShipManager.Instance._playerSpeed.AddModifier(new StatModifier(value,modType));
            else
            {
                Debug.Log("敵の速度増加:無効");
            }
        }
    }
}

