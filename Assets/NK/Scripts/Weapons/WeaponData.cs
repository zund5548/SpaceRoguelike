using UnityEngine;
using Ships;
using System;
namespace Weapons
{
    //SubclassSelectorで各々のshipDataに設定
    [Serializable]
    public abstract class WeaponData
    {
        public virtual void ShootAction(GameObject applyingdShipObject,Ship applyingShip){}
    }
}

