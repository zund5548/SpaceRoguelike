using Ships;
using UnityEngine;
using Managers;
using Stats;
using System;
using System.Collections.Generic;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ValueModify")]
    [Serializable]
    public class ValueModify:ItemEffect
    {
        public float value;
        public StatType statType;
        public ModType modType;
        public override void OnApply()
        {
            List<GameObject> shipObjectList = isPlayers?ShipManager.Instance.playerShipObjectList:ShipManager.Instance.enemyShipObjectList;
            //List<GameObject> shipObjectList = ShipManager.Instance.enemyShipObjectList;
            foreach(var shipObject in shipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                //もうstatは入っている
                ship.GetStat(statType).AddModifier(new StatModifier(value,modType));
                ship.SetCurrentSPHP();
            }
        }
    }
}

