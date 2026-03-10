using UnityEngine;
using Ships;
using System;
namespace Weapons
{
    [Serializable]
    public abstract class WeaponData
    {
        public virtual void ShootAction(GameObject applyingdShipObject,Ship applyingShip){}
    }
}

