using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/BulletStatModify")]
    [Serializable]
    public class BulletStatModify:ItemEffect
    {
        public float value;
        public BulletStatSet.BulletStatType bulletShotStatType;
        public ModType modType;
        public override void OnApply()
        {
            foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                if(ship.uniqueStatController.GetUniqueStat<BulletStatSet>() == null)ship.uniqueStatController.AddUniqueStat(new BulletStatSet{});
                ship.uniqueStatController.GetUniqueStat<BulletStatSet>().GetStat(bulletShotStatType).AddModifier(new StatModifier(value,modType));
                ship.SetCurrentSPHP();
            }
        }
    }
}