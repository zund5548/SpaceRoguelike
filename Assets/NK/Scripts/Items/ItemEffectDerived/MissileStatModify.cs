using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/MissileStatModify")]
    [Serializable]
    public class MissileStatModify:ItemEffect
    {
        public float value;
        public MissileStatSet.MissileStatType missileShotStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<MissileStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new MissileStatSet{});
                ship.uniqueStatController.GetUniqueStat<MissileStatSet>().GetStat(missileShotStatType).AddModifier(new StatModifier(value,modType)); 
                //もうstatは入っている
                ship.SetCurrentSPHP();
            }
        }
    }
}

