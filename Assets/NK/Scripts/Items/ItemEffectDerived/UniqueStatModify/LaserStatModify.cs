using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Items;
using Stats;
using UnityEngine;
using Managers;
using Ships;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/LaserStatModify")]
    [Serializable]
    public class LaserStatModify:ItemEffect
    {
        public float value;
        public LaserStatSet.LaserStatType laserShotStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<LaserStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new LaserStatSet{});
                ship.uniqueStatController.GetUniqueStat<LaserStatSet>().GetStat(laserShotStatType).AddModifier(new StatModifier(value,modType));
                //ship.SetCurrentSPHP();
            }
        }
    }
}