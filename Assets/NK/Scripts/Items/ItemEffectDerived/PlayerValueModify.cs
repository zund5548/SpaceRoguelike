using Ships;
using UnityEngine;
using Managers;
using Stats;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/PlayerValueModify")]
    [Serializable]
    public class PlayerValueModify:ItemEffect
    {
        public float value;
        public StatType statType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                // switch(statType)
                // {
                //     case StatType.Power:
                //         shipObject.GetComponent<Ship>().currentPower.AddModifier(new StatModifier(value,modType));
                //         break;
                //     case StatType.Shield:
                //         shipObject.GetComponent<Ship>().maxShieldPoint.AddModifier(new StatModifier(value,modType));
                //         break;
                //     case StatType.Hull:
                //         shipObject.GetComponent<Ship>().maxHullPoint.AddModifier(new StatModifier(value,modType));
                //         break;
                // }
                var ship = shipObject.GetComponent<Ship>();
                //もうstatは入っている
                ship.GetStat(statType).AddModifier(new StatModifier(value,modType));
                ship.SetCurrentSPHP();
            }
        }
    }
}

