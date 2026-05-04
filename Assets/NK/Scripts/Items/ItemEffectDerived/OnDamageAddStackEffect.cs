using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;
using Managers;
using Ships;
namespace Items
{
    //ダメージを与えたとき、被弾した相手に確率でスタック付与
    [CreateAssetMenu(menuName = "ItemEffect/OnDamageAddStackEffect")]
    [Serializable]
    public class OnDamageAddStackEffect:ItemEffect
    {
        [SerializeReference,SubclassSelector]
        public StackEffect stackEffect;
        public override void OnApply()
        {
            List<Ship> shipList = isPlayers?ShipManager.Instance.playerShipList:ShipManager.Instance.enemyShipList;
            var type = stackEffect.GetType();
            foreach(var ship in shipList)
            {
                ship.shipEventController.OnDamaging
                    .Subscribe(sde =>
                    {
                        AddSelectedStack(stackEffect,sde);
                    })
                    .AddTo(ship.gameObject);
            }
        }
        private void AddSelectedStack(StackEffect stackEffect,ShipEventController.ShipAttackEvent sae)
        {
            switch(stackEffect)
            {
                case AdditionalMissileStackEffect:
                    sae.targetShip.stackEffectController.AddStackNum<AdditionalMissileStackEffect>(sae.dealerShip,sae.targetShip,1);
                    break;
                case CrackStackEffect:
                    sae.targetShip.stackEffectController.AddStackNum<CrackStackEffect>(sae.dealerShip,sae.targetShip,1);
                    break;
                case SurgeStackEffect:
                    sae.targetShip.stackEffectController.AddStackNum<SurgeStackEffect>(sae.dealerShip,sae.targetShip,1);
                    break;
            }
        }
    }
}