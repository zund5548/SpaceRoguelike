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
        //ダメージを受けたとき
        private static Subject<ShipDamageEvent> onDamage = new Subject<ShipDamageEvent>();
        public static IObservable<ShipDamageEvent> OnDamage => onDamage;
        public struct ShipDamageEvent
        {
            public Ship ship;
            public Ship dealingShip;
            public int delatDamageValue;
        }
        public void PublishDamaged(ShipDamageEvent shipDamageEvent)
        {
            onDamage.OnNext(shipDamageEvent);
            //Debug.Log("a");
        }
    }
}

