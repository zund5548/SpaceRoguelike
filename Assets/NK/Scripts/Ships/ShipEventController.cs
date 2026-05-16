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
            OnHit,
            OnKilled,
            OnCritical,
        }
        public struct ShipAttackEvent
        {
            public Ship targetShip;//攻撃を受けた船
            public Ship dealerShip;//攻撃を与えた船
            public int dealtDamageValue;//与えたダメージ
            public ShipAttackEvent(Ship targetShip,Ship dealerShip,int dealtDamageValue)
            {
                this.targetShip = targetShip;
                this.dealerShip = dealerShip;
                this.dealtDamageValue = dealtDamageValue;
            }
            public ShipAttackEvent(Ship targetShip,Ship dealerShip)
            {
                this.targetShip = targetShip;
                this.dealerShip = dealerShip;
                dealtDamageValue = 0;
            }
        }


        // public IObservable<ShipAttackEvent> GetEventByCategory(EventCategory eventCategory)
        // {
        //     return eventCategory switch
        //     {
        //         EventCategory.OnDamaging => onDamaging,
        //         EventCategory.OnShoot => onShoot,
        //         EventCategory.OnHit => onHit,
        //         EventCategory.OnKilled => onKilling,
        //         EventCategory.OnCritical => onCritical,
        //         _ => null
        //     };
        // }

        /// <summary>
        /// ダメージを受けたとき
        /// </summary>
        private Subject<ShipAttackEvent> onDamaging = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnDamaging => onDamaging;
        public void PublishDamaging(ShipAttackEvent shipAttackEvent)
        {
            onDamaging.OnNext(shipAttackEvent);
        }

        /// <summary>
        /// 砲撃したとき
        /// </summary>
        private Subject<ShipAttackEvent> onShoot = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnShoot => onShoot;
        public void PublishShoot(ShipAttackEvent shipAttackEvent)
        {
            onShoot.OnNext(shipAttackEvent);
        }

        /// <summary>
        /// この艦船の攻撃が当たった時
        /// </summary>
        private Subject<ShipAttackEvent> onHit = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnHit => onHit;
        public void PublishHit(ShipAttackEvent shipDamageEvent)
        {
            onHit.OnNext(shipDamageEvent);
        }

        /// <summary>
        /// この艦船が敵を倒したとき
        /// </summary>
        private Subject<ShipAttackEvent> onKilling = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnKilling => onKilling;
        public void PublishKilling(ShipAttackEvent shipDamageEvent)
        {
            onKilling.OnNext(shipDamageEvent);
        }

        /// <summary>
        /// この艦船がクリティカル攻撃を出したとき
        /// </summary>
        private Subject<ShipAttackEvent> onCritical = new Subject<ShipAttackEvent>();
        public IObservable<ShipAttackEvent> OnCritical => onCritical;
        public void PublishCritical(ShipAttackEvent shipDamageEvent)
        {
            onCritical.OnNext(shipDamageEvent);
        }
    }
}

