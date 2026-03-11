using System.Collections.Generic;
using Items;
using UnityEngine;
namespace Items
{
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        [SerializeReference]
        public List<ItemEffect> itemEffects;
    }
}

