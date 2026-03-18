using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Ships;
using Projectiles;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Weapons
{
    [Serializable]
    public class ChainShot : WeaponData
    {
        public GameObject ChianLineObject;
        public float range = 10f;
        public float chainRange = 5f;
        public float shotInterval = 2f;
        public float chainInterval = 0.1f;
        public int maxChainNum = 5;
        private void  SetChainLine(GameObject start,GameObject end)
        {
            
            var chain = UnityEngine.Object.Instantiate(ChianLineObject,start.transform.position,Quaternion.identity);
            var lr = chain.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            Vector2 startPos = start.transform.position;
            Vector2 endPos = end.transform.position;
            chain.UpdateAsObservable()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(0.2f)))
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
        private bool IsSame(List<GameObject> shipObjectList,GameObject targetShipObject)
        {
            foreach(var shipObject in  shipObjectList)
            {
                if(shipObject == targetShipObject)return true;
            }
            return false;
        }
        private void ShootChain(Ship applyingShip,GameObject startShipObject,GameObject endShipObject,List<GameObject> targetShips,float chainInterval,int chainNum)
        {
            if(chainNum == 0)return;
            endShipObject.GetComponent<Ship>().DealDamage((int)applyingShip.currentPower.Value);
            SetChainLine(startShipObject,endShipObject);
            float mindist = Mathf.Infinity;
            GameObject nearest = null;
            foreach(var opponetShipObject in applyingShip.opponentShipObjectList)
            {
                if(IsSame(targetShips,opponetShipObject))continue;
                float dist = Vector2.Distance(startShipObject.transform.position,opponetShipObject.transform.position);
                if(mindist > dist)
                {
                    mindist = dist;
                    nearest = opponetShipObject;
                }
            }
            if(nearest == null)return;
            if(mindist > chainRange)return;
            targetShips.Add(nearest);
            applyingShip.UpdateAsObservable()
                .Delay(TimeSpan.FromSeconds(chainInterval))
                .Take(1)
                .Subscribe(_=>
                {   if(!endShipObject || !nearest)return;
                    ShootChain(applyingShip,endShipObject,nearest,targetShips,chainInterval,chainNum - 1);
                })
                .AddTo(applyingShip);
        }

        public override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            if(applyingShip == null)return;
            var trueSir = applyingShip.shotIntervalReduction.Value < MAX_ShotIntervalReduction ? applyingShip.shotIntervalReduction.Value : MAX_ShotIntervalReduction;
            applyingShipObject.UpdateAsObservable()
                .DelaySubscription(TimeSpan.FromSeconds(UnityEngine.Random.Range(0,0.5f)))
                .ThrottleFirst(TimeSpan.FromSeconds(shotInterval * (100f - trueSir)/100f))
                .Subscribe(_ =>
                {
                    applyingShip.GetNearestOpponet();
                    if(!applyingShipObject || !applyingShip.targetObject)return;
                    if(Vector2.Distance(applyingShipObject.transform.position,applyingShip.targetObject.transform.position) > range)return;
                    ShootChain(applyingShip,applyingShipObject,applyingShip.targetObject,new List<GameObject>{applyingShip.targetObject},chainInterval,maxChainNum);
                    //ShootChain(applyingdShipObject,applyingShip);
                })
                .AddTo(applyingShipObject);
        }
    }
}

