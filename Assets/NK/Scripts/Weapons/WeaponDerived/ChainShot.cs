using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using Stats;
using Items;

namespace Weapons
{
    [Serializable]
    public class ChainShot : WeaponData
    {
        //敵の間を連鎖してダメージを与える電撃を撃つ
        //連鎖した敵には1/2倍のダメージ
        public GameObject ChianLineObject;
        public float range = 10f;
        public float chainInterval = 0.2f;
        [Header("unique stat")]
        public int maxChainNum = 3;
        public float chainRange = 5f;
        public override void SetUniqueStat(Ship applyingShip)
        {
            if(applyingShip.uniqueStatController.GetUniqueStat<ChainStatSet>() != null)return;
            applyingShip.uniqueStatController.AddUniqueStat(
            new ChainStatSet
            {
                chainNum = new(maxChainNum),
                chainRange = new(chainRange)
            });
        }
        public override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            SetWeaponPrefab();
            float currentShotInterval = applyingShip.shotInterval.Value;
            //var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                //.ThrottleFirst(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .ThrottleFirst(TimeSpan.FromSeconds(currentShotInterval))
                .Subscribe(_ =>
                {
                    
                    var targetShipObject = applyingShip.GetNearestOpponet();
                    if(!applyingShipObject || !targetShipObject)return;
                    //if(targetShipObject && targetShipObject.tag == "PlayerAnchor")return;
                    if(Vector2.Distance(applyingShipObject.transform.position,applyingShip.GetNearestOpponet().transform.position) > range)return;
                    //ShootChain(applyingShip,applyingShipObject,applyingShip.targetObject,new List<GameObject>{applyingShip.targetObject},chainInterval,maxChainNum);
                    Shoot(applyingShipObject,applyingShip);
                })
                .AddTo(applyingShipObject);
        }
        public override void Shoot(GameObject applyingdShipObject, Ship applyingShip)
        {
            if(applyingShip.isSurged)return;
            // GameObject startShipObject = applyingShip.targetObject;
            // GameObject endShipObject = null;
            GameObject startShipObject = applyingdShipObject;
            GameObject endShipObject = applyingShip.GetNearestOpponet();
            //bool isChainNext = true;
            List<GameObject> targetObjectList = new List<GameObject>{endShipObject};
            Observable.Interval(System.TimeSpan.FromSeconds(chainInterval))
                .Take(maxChainNum)
                .Subscribe(x =>
                {
                    if(x != 0)endShipObject = FindNearestOf(startShipObject,targetObjectList,applyingShip);
                    
                    if(endShipObject)
                    {
                        var targetShip = endShipObject.GetComponent<Ship>();
                        if(startShipObject == applyingdShipObject)targetShip.DealDamage((int)applyingShip.currentPower.Value,false,applyingShip);
                        else targetShip.DealDamage((int)(applyingShip.currentPower.Value / 2f),false,applyingShip);

                        bool enableAddSurge = applyingShip.uniqueStatController.GetUniqueStat<ChainStatSet>().enableAddSurge.IsAble;
                        if(enableAddSurge)targetShip.stackEffectController.AddStack<SurgeStackEffect>(applyingShip,targetShip);

                        targetObjectList.Add(endShipObject);
                        SetChainLine(startShipObject,endShipObject);
                        startShipObject = endShipObject;
                    }
                    else return;
                })
                .AddTo(applyingdShipObject);
        }
        /// <summary>オブジェクト間に線を引く</summary>
        private void  SetChainLine(GameObject start,GameObject end)
        {   
            var chain = UnityEngine.Object.Instantiate(ChianLineObject,start.transform.position,Quaternion.identity);
            var lr = chain.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            Vector2 startPos = start.transform.position;
            Vector2 endPos = end.transform.position;
            chain.UpdateAsObservable()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(chainInterval*2f)))
                .Subscribe(_ =>
                {
                    if(start)startPos = start.transform.position;
                    if(end)endPos = end.transform.position;
                    lr.SetPosition(0,startPos);
                    lr.SetPosition(1,endPos);
                },
                ()=>
                {
                    UnityEngine.Object.Destroy(chain);
                })
                .AddTo(chain);
        }
        private bool IsInTarget(List<GameObject> shipObjectList,GameObject targetShipObject)
        {
            foreach(var shipObject in  shipObjectList)
            {
                if(shipObject == targetShipObject)return true;
            }
            return false;
        }
        private GameObject FindNearestOf(GameObject centerShipObject,List<GameObject> targetShipObjects,Ship applyingShip)
        {
            GameObject result = null;
            float mindist = Mathf.Infinity;
            foreach(var ship in  applyingShip.opponentShipObjectList)
            {
                if(!centerShipObject || !ship)continue;
                float dist = Vector2.Distance(centerShipObject.transform.position,ship.transform.position);
                if(IsInTarget(targetShipObjects,ship))continue;
                if(ship == centerShipObject.GetComponent<Ship>())continue;
                // if(dist < mindist && dist < range)
                // {
                //     mindist = dist;   
                //     result = ship;
                // }
                if(dist < mindist)
                {
                    if(centerShipObject.GetComponent<Ship>() == applyingShip)
                    {
                        if(dist > range)continue;
                    }
                    else
                    {
                        if(dist > chainRange)continue;
                    }
                    
                    mindist = dist;   
                    result = ship;
                }
            }
            return result;
        }

    }
}

