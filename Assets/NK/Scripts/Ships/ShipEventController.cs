using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using Ships;
namespace Ships
{
    public class ShipEventController
    {
        //ダメージを受けたとき
        private Subject<ShipDamagingEvent> onDamage = new Subject<ShipDamagingEvent>();
        public IObservable<ShipDamagingEvent> OnDamage => onDamage;
        public struct ShipDamagingEvent
        {
            public Ship targetShip;//受けた船
            public Ship dealingShip;//与えた船
            public int dealtDamageValue;//与えたダメージ
        }
        public void PublishDamaging(ShipDamagingEvent shipDamageEvent)
        {
            onDamage.OnNext(shipDamageEvent);
        }
        // //砲撃したとき
        private Subject<ShipShotEvent> onShoot = new Subject<ShipShotEvent>();
        public IObservable<ShipShotEvent> OnShoot => onShoot;
        public struct ShipShotEvent
        {
            public Ship  shootingShip;//砲撃した船
        }
        public void PublishShoot(ShipShotEvent shipShotEvent)
        {
            onShoot.OnNext(shipShotEvent);
        }
        // 攻撃がこの艦船にヒットしたとき
        private Subject<ShipDamagingEvent> onHit = new Subject<ShipDamagingEvent>();
        public IObservable<ShipDamagingEvent> OnHit => onHit;
        public void PublishHit(ShipDamagingEvent shipDamageEvent)
        {
            onHit.OnNext(shipDamageEvent);
        }
    }
}

