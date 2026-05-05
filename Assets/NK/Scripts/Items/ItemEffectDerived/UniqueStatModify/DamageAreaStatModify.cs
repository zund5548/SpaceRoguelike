using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/DamageAreaStatModify")]
    [Serializable]
    public class DamageAreaStatModify:ItemEffect
    {
         public float value;
        public DamageAreaStatSet.DamageAreaStatType damageAreaStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<DamageAreaStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new DamageAreaStatSet{});
                ship.uniqueStatController.GetUniqueStat<DamageAreaStatSet>().GetStat(damageAreaStatType).AddModifier(new StatModifier(value,modType)); 
                //ship.SetCurrentSPHP();
            }
        }
    }
}

