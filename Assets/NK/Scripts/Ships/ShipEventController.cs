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
        private Subject<ShipDamageEvent> onDamage = new Subject<ShipDamageEvent>();
        public IObservable<ShipDamageEvent> OnDamage => onDamage;
        public struct ShipDamageEvent
        {
            public Ship targetShip;//受けた船
            public Ship dealingShip;//与えた船
            public int delatDamageValue;//与えたダメージ
        }
        public void PublishDamaged(ShipDamageEvent shipDamageEvent)
        {
            onDamage.OnNext(shipDamageEvent);
        }
        // //攻撃したとき
        private Subject<ShipShotEvent> onShoot = new Subject<ShipShotEvent>();
        public IObservable<ShipShotEvent> OnShoot => onShoot;
        public struct ShipShotEvent
        {
            public Ship  dealingShip;//攻撃した船
        }
        public void PublishShoot(ShipShotEvent shipShotEvent)
        {
            onShoot.OnNext(shipShotEvent);
        }
        // 攻撃がヒットしたとき
        private Subject<ShipDamageEvent> onHit = new Subject<ShipDamageEvent>();
        public IObservable<ShipDamageEvent> OnHit => onHit;
        public void PublishHit(ShipDamageEvent shipDamageEvent)
        {
            onHit.OnNext(shipDamageEvent);
        }
    }
}

