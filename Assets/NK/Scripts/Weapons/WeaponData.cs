using UnityEngine;
using Ships;
using System;
namespace Weapons
{
    //SubclassSelectorで各々のshipDataに設定
    [Serializable]
    public abstract class WeaponData
    {
        /// <summary>射撃間隔は90%以上早くならない</summary>
        public const float MAX_ShotIntervalReduction = 90f;
        public virtual void ShootAction(GameObject applyingdShipObject,Ship applyingShip){}
    }
}

