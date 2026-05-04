using UnityEngine;
using Ships;
using System;
namespace Weapons
{
    //SubclassSelectorで各々のshipDataに設定
    [Serializable]
    public abstract class WeaponData
    {
        public string weaponName;
        /// <summary>射撃間隔はこの数値(percent)以上早くならない</summary>
        public const float MAX_ShotIntervalReduction = 90f;
        public virtual void ShootAction(GameObject applyingShipObject,Ship applyingShip){}
        public virtual void Shoot(GameObject applyingShipObject,Ship applyingShip){}
        public abstract void SetUniqueStat(Ship applyingShip);
    }
}

