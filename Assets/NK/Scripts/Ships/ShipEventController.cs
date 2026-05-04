using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using Ships;
using Items;
namespace Ships
{
    public class ShipEventController
    {
        public enum EventCategory
        {
            OnDamaging,
            OnShoot,
            OnHit
        }
        public struct ShipAttackEvent
        {
            public Ship targetShip;//攻撃を受けた船
            public Ship dealerShip;//攻撃を与えた船
            public int dealtDamageValue;//与えたダメージ
        }


        public IObservable<ShipAttackEvent> GetEventByCategory(EventCategory eventCategory)
        {
            return eventCategory switch
            {
                EventCategory.OnDamaging => onDamaging,
                EventCategory.OnShoot => onShoot,
                EventCategory.OnHit => onHit,
                _ => null
            };
        }

        //ダメージを受けたとき
        private Subject<ShipAttackEvent> onDamaging = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnDamaging => onDamaging;
        public void PublishDamaging(ShipAttackEvent shipAttackEvent)
        {
            onDamaging.OnNext(shipAttackEvent);
        }

        //砲撃したとき
        private Subject<ShipAttackEvent> onShoot = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnShoot => onShoot;
        public void PublishShoot(ShipAttackEvent shipAttackEvent)
        {
            onShoot.OnNext(shipAttackEvent);
        }

        // 攻撃がこの艦船にヒットしたとき
        private Subject<ShipAttackEvent> onHit = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnHit => onHit;
        public void PublishHit(ShipAttackEvent shipDamageEvent)
        {
            onHit.OnNext(shipDamageEvent);
        }
    }
}

