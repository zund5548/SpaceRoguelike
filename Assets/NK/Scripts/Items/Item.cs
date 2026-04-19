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
        [SerializeReference]
        public List<ItemEffect> itemEffectList;
        /// <summary>すべてのitemEffectの説明をstringで返す</summary>
        public string GetItemDescription()
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

