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
                var ship = shipObject.GetComponent<Ship>();
                //もうstatは入っている
                ship.GetStat(statType).AddModifier(new StatModifier(value,modType));
                ship.SetCurrentSPHP();
            }
        }
    }
}

