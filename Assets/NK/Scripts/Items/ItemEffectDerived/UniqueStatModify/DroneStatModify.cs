using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/DroneStatModify")]
    [Serializable]
    public class DroneStatModify:ItemEffect
    {
        public float value;
        public DroneStatSet.DroneStatType droneStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<DroneStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new DroneStatSet{});
                ship.uniqueStatController.GetUniqueStat<DroneStatSet>().GetStat(droneStatType).AddModifier(new StatModifier(value,modType)); 
                //もうstatは入っている
                ship.SetCurrentSPHP();
            }
        }
    }
}

