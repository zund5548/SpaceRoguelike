using UnityEngine;
using UniRx;
using System;
using Managers;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ReflectionEffect")]
    [Serializable]
    public class ReflectionEffect:ItemEffect
    {
        public int power;
        public override void OnApply()
        {
            // EventManager.OnDamage
            //     .Subscribe(damageEvent =>
            //     {
            //         //反射時はONDamageを発行しない
            //         damageEvent.dealingShip.DealDamage(power);
            //     })
            //     .AddTo(EventManager.Instance);
        }
    }
}