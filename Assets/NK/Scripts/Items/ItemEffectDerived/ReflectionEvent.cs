using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using Ships;
using TMPro;
using System;
using System.Collections;
using Managers;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ReflectionEffect")]
    [Serializable]
    public class ReflectionEffect:ItemEffect
    {
        public int power;
        public override void OnApply(Ship appliedShip)
        {
            
            EventManager.OnDamage
                .Subscribe(damageEvent =>
                {
                    Debug.Log(damageEvent.dealingShip.shipData.name+"/"+damageEvent.ship.shipData.name);
                    //反射時はONDamageを発行しない
                })
                .AddTo(appliedShip);
        }
    }
}