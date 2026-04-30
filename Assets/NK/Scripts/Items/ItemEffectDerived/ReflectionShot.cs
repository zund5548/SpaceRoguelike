using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections.Generic;
using Managers;
using Unity.VisualScripting;
using Ships;
namespace Items
{
    [CreateAssetMenu(menuName = "ItemEffect/ReflectionShot")]
    [Serializable]
    public class ReflectionShot:ItemEffect
    {
        public override void OnApply()
        {
            // EventManager.OnDamage
            List<Ship> shipList = isPlayers?ShipManager.Instance.playerShipList:ShipManager.Instance.enemyShipList;
            foreach(var ship in shipList)
            {
                ship.shipEventController.OnShoot
                    .Subscribe(_ =>
                    {
                        ship.stackEffectController.AddStackNum<AdditionalMissileStackEffect>(ship,ship,1);
                    })
                    .AddTo(ship.gameObject);
            }
        }
    }
}