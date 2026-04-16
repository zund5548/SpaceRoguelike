using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Maps;
using Ships;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
namespace Items
{
    /// <summary>スタックが溜まると数秒移動不可</summary>
    public class RadiationStackEffect:StackEffect
    {
        public int threshold = 4;
        public float dotDamage = 10f;
        public float dotInterval = 0.5f;
        public float lastTime = 5f;
        public override void OnStackChanged()
        {
            Debug.Log(stackNum);
            if(stackNum >= threshold)
            {
                stackNum %= threshold;
                if(isAbletoAdd)isAbletoAdd = false;
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(dotInterval))
                    .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(lastTime)))
                    .Subscribe(_ =>
                    {
                        ownerShip.DealDamage((int)dotDamage,false);
                    })
                    .AddTo(ownerShip);
            }
        }
    }
}

