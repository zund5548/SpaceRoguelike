using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
namespace Items
{
    //アイテムは条件を満たした味方艦すべてにItemEffectを与える
    [CreateAssetMenu(menuName = "Item")]
    [Serializable]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        public int itemTier;//大きいほどいいもの
        public int maxNum = 1;//最大所持数
        [SerializeReference]
        public List<ItemEffect> itemEffectList;
        public string GetDescription()
        {
            string result = "";
            foreach(var itemEffect in itemEffectList)
            {
                result += "・" + itemEffect.description + "\n";
            }
            return result;
        }
    }
}

