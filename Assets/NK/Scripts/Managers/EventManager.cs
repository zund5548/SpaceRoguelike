using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using Ships;
namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance{get;private set;}     
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            
        }
        //ステージをクリアしたとき
        private static Subject<Unit> onStageClear = new Subject<Unit>();
        public static IObservable<Unit> OnStageClear => onStageClear;
        public void PublishClear()
        {
            onStageClear.OnNext(Unit.Default);
        }
        //失敗したとき
        private static Subject<Unit> onStageFalse = new Subject<Unit>();
        public static IObservable<Unit> OnStageFalse => onStageFalse;
        public void PublishFalse()
        {
            onStageFalse.OnNext(Unit.Default);
        }
        //ダメージを受けたとき
        private static Subject<ShipDamageEvent> onDamage = new Subject<ShipDamageEvent>();
        public static IObservable<ShipDamageEvent> OnDamage => onDamage;
        public struct ShipDamageEvent
        {
            public Ship targetShip;//受けた船
            public Ship dealingShip;//与えた船
            public int delatDamageValue;//与えたダメージ
        }
        public void PublishDamaged(ShipDamageEvent shipDamageEvent)
        {
            onDamage.OnNext(shipDamageEvent);
            //Debug.Log("a");
        }
        //攻撃したとき
        private static Subject<ShipShotEvent> onShoot = new Subject<ShipShotEvent>();
        public static IObservable<ShipShotEvent> OnShoot => onShoot;
        public struct ShipShotEvent
        {
            public Ship  dealingShip;//攻撃した船
        }
        public void PublishShoot(ShipShotEvent shipShotEvent)
        {
            onShoot.OnNext(shipShotEvent);
        }
        //攻撃がヒットしたとき
        // private static Subject<ShipDamageEvent> onHit = new Subject<ShipDamageEvent>();
        // public static IObservable<ShipDamageEvent> OnHit => onHit;
        // public void PublishHit(ShipDamageEvent shipDamageEvent)
        // {
        //     onHit.OnNext(shipDamageEvent);
        // }

    }
}

