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
using Weapons;

namespace Items
{
    /// <summary>スタックが溜まると追加で砲撃</summary>
    public class AdditionalMissileStackEffect:StackEffect
    {
        public int threshold = 5;
        public MissileShot missileShot = null;
        private bool isFirst = true;
        public override void OnStackChanged()
        {   
            if(isFirst)
            {
                isFirst = false;
                missileShot = new MissileShot
                {
                    projectile = (GameObject)Resources.Load("projectile"),
                    explosion = (GameObject)Resources.Load("explosion"),
                    range = 100f,
                    projectileSpeed = 1f,
                    //shotInterval
                    //burstNum
                    hitTime = 0.8f,
                    errorRadius = 0f,
                    explosionRadius = 2f,
                };
            }
            if(stackNum >= threshold)
            {
                stackNum %= threshold;
                missileShot.Shoot(ownerShip.gameObject,ownerShip);
                //ship.stackEffectController.AddStackNum<AdditionalShotStackEffect>(ship,1);
            }
        }
    }
}
