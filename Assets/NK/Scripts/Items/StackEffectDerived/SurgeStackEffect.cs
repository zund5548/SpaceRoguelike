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
    /// <summary>スタックが溜まると移動不可</summary>
    [Serializable]
    public class SurgeStackEffect:StackEffect
    {
        public int threshold = 5;
        public float moveDisableTime = 3f;
        public GameObject effectIconObject;
        public override void OnStackChanged()
        {
            Debug.Log(stackNum);
            if(stackNum >= threshold)
            {
                stackNum %= threshold;
                if(ownerShip.isAbleToMove)ownerShip.isAbleToMove = false;
                if(isAbletoAdd)isAbletoAdd = false;
                GameObject iconObject = null;
                if(effectIconObject)iconObject = UnityEngine.Object.Instantiate(effectIconObject,ownerShip.transform.position,Quaternion.identity);
                Observable.Timer(TimeSpan.FromSeconds(moveDisableTime))
                    .Subscribe(_ =>
                    {
                        if(iconObject)UnityEngine.Object.Destroy(iconObject);
                        ownerShip.isAbleToMove = true;
                        isAbletoAdd = true;
                    })
                    .AddTo(ownerShip);
                ownerShip.gameObject.OnDestroyAsObservable()
                    .Subscribe(_ =>
                    {
                        if(iconObject)UnityEngine.Object.Destroy(iconObject);
                    });
            }
        }
    }
}

