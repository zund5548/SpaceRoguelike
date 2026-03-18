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
    public class CrackStackEffect:StackEffect
    {
        public int threshold = 5;
        public float constantDamage = 100f;
        public override void OnStackChanged()
        {
            Debug.Log(stackNum);
            if(stackNum >= threshold)
            {
                stackNum %= threshold;
                ownerShip.DealDamage((int)(constantDamage + (ownerShip.currentHullPoint + ownerShip.currentShieldPoint) * 0.1f));
            }
        }
    }
}

