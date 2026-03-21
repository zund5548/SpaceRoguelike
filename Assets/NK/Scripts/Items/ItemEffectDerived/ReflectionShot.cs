using UnityEngine;
using UniRx;
using System;
using Managers;
using Unity.VisualScripting;

namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ReflectionShot")]
    [Serializable]
    public class ReflectionShot:ItemEffect
    {
        public override void OnApply()
        {
            EventManager.OnDamage
                .Subscribe(damageEvent =>
                {
                    //反射時はONDamageを発行しない
                    damageEvent.dealingShip.stackEffectController.AddStackNum<AdditionalMissileStackEffect>(damageEvent.dealingShip,1);
                })
                .AddTo(EventManager.Instance);
        }
    }
}