using System;
using Ships;
using UnityEngine;
namespace Items
{
    [Serializable]
    public class ItemEffect:ScriptableObject
    {
        public virtual void OnApply(){} 
        //public virtual void OnRemove(Ship appliedShip){}
    }
}

