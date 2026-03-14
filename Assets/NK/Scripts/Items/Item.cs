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
        public string description;
        [SerializeReference]
        public List<ItemEffect> itemEffectList;
    }
}

