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
        private static Subject<Unit> onStageFail = new Subject<Unit>();
        public static IObservable<Unit> OnStageFail => onStageFail;
        public void PublishFail()
        {
            onStageFail.OnNext(Unit.Default);
        }
        //ボスを倒したとき、
        private static Subject<Unit> onBossBeat= new Subject<Unit>();
        public static IObservable<Unit> OnBossBeat => onBossBeat;
        public void PublishBossBeat()
        {
            onBossBeat.OnNext(Unit.Default);
        }
    }   
}

