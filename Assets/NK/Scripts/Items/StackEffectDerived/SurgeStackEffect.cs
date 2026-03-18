using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Maps;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
namespace Items
{
    /// <summary>スタックが溜まるとダメージ</summary>
    public class SurgeStackEffect:StackEffect
    {
        public int threshold = 5;
        public float cantMoveTime = 3f;
        public override void OnStackChanged()
        {
            Debug.Log(stackNum);
            if(stackNum >= threshold)
            {
                stackNum %= threshold;
                if(ownerShip.isAbleToMove)ownerShip.isAbleToMove = false;
                if(isAbletoAdd)isAbletoAdd = false;
                Observable.Timer(TimeSpan.FromSeconds(cantMoveTime))
                    .Subscribe(_ =>
                    {
                        ownerShip.isAbleToMove = true;
                        isAbletoAdd = true;
                    })
                    .AddTo(ownerShip);
            }
        }
    }
}

