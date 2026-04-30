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
    [Serializable]
    public class CrackStackEffect:StackEffect
    {
        public int threshold = 5;
        //public float constantDamage = 1f;
        public GameObject effectIconObject = null;
        public override void OnStackChanged()
        {
            if(!effectIconObject)effectIconObject = (GameObject)Resources.Load("Crack");
            if(stackNum >= threshold)
            {
                stackNum %= threshold;
                ownerShip.DealDamage((int)(dealerShip.currentPower.Value * 20f/100f),false);
                GameObject iconObject = null;
                if(effectIconObject)iconObject = UnityEngine.Object.Instantiate(effectIconObject,ownerShip.transform.position,Quaternion.identity);
                iconObject.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        var sr = iconObject.GetComponent<SpriteRenderer>();
                        Color color = sr.color;
                        var newColor = new Color(color.r,color.g,color.b,color.a);
                        newColor.a -= Time.deltaTime;
                        Debug.Log(newColor.a);
                        sr.color = newColor;
                    })
                    .AddTo(iconObject);
                Observable.Timer(TimeSpan.FromSeconds(1f))
                    .Subscribe(_ =>
                    {
                        if(iconObject)UnityEngine.Object.Destroy(iconObject);
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

