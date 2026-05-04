using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/UniqueStatModify")]
    [Serializable]
    public class UniqueStatModify:ItemEffect
    {
        public float value;
        [SerializeReference,SubclassSelector]
        public UniqueStatCollection uniqueStatCollection;
        public ModType modType;
        public override void OnApply()
        {   
            // foreach(var shipObject in  ShipManager.Instance.playerShipObjectList)
            // {
            //     var ship = shipObject.GetComponent<Ship>();
            //     //statSetがshipにあるか確認　なかったら追加
            //     if(ship.uniqueStatController.GetUniqueStatByCollection<UniqueStatCollection>() == null)ship.uniqueStatController.AddUniqueStat(uniqueStatCollection.GetStatSet());
            //     //GetUniqueStatを使って、どのStatSetのどのstatTypeを強化するのか指定する
            //     ship.uniqueStatController.GetUniqueStatByCollection<UniqueStatCollection>().GetStatSet().GetStat().AddModifier(new StatModifier(value,modType));
            // }
        }
    }
}

