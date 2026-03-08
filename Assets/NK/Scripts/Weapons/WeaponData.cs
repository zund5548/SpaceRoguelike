using UnityEngine;
using Ships;
namespace Weapons
{
    public class WeaponData : ScriptableObject
    {
        public virtual void ShootAction(GameObject applyingdShipObject,Ship applyingShip){}
    }
}

