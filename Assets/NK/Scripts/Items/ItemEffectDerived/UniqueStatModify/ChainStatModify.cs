using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;

namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ChainStatModify")]
    public class ChainStatModify:ItemEffect
    {
        public float value;
        public ChainStatSet.ChainStatType chainStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<ChainStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new ChainStatSet{});
                ship.uniqueStatController.GetUniqueStat<ChainStatSet>().GetStat(chainStatType).AddModifier(new StatModifier(value,modType)); 
                //ship.SetCurrentSPHP();
            }
        }
    }
}