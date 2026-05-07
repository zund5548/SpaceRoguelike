using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
using Items;

namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/RailgunStatModify")]
    [Serializable]
    public class RailgunStatModify:ItemEffect
    {
        public float value;
        public RailgunStatSet.RailGunStatType railgunShotStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<RailgunStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new RailgunStatSet{});
                ship.uniqueStatController.GetUniqueStat<RailgunStatSet>().GetStat(railgunShotStatType).AddModifier(new StatModifier(value,modType));
                //ship.SetCurrentSPHP();
            }
        }
    }
}