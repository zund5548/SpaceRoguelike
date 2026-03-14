using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
namespace Items
{
    [CreateAssetMenu(menuName = "Item")]
    [Serializable]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        [SerializeReference]
        public List<ItemEffect> itemEffectList;
    }
}

