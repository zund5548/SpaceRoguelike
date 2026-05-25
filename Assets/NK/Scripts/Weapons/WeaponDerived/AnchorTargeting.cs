using UnityEngine;
using Ships;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
using Managers;
using Stats;
using System;
using Items;
namespace Weapons
{
    [Serializable]
    public class AnchorTargeting:WeaponData
    {
        public GameObject ChianLineObject;
        public override void SetUniqueStat(Ship applyingShip)
        {
            
        }
        public override void ShootAction(GameObject applyingShipObject,Ship applyingShip)
        {
            var anchor = ShipManager.Instance.GetAnchorObject();
            if(!anchor)return;
            //艦船の動きを変える
            var v = anchor.transform.position - applyingShipObject.transform.position;
            float rad  = Mathf.Atan2(v.y,v.x);
            applyingShip.shipMoveStream.Dispose();
            // applyingShip.shipMoveStream = applyingShipObject.UpdateAsObservable()
            //     .Subscribe(_ =>
            //     {
            //         if(Vector2.Distance(applyingShipObject.transform.position,anchor.transform.position) > 1f)
            //         {
            //             applyingShipObject.transform.position += (Vector3)(applyingShip.shipData.speed * Time.deltaTime * new Vector2(Mathf.Cos(rad),Mathf.Sin(rad)));
            //         }
            //         else
            //         {
                        
            //         }
            //     })
            //     .AddTo(applyingShip);
            Observable.Timer(TimeSpan.Zero)
                .SelectMany(_ =>
                {
                    var everyFrame = Observable.EveryUpdate()
                        .Do(_=>
                        {
                            if(Vector2.Distance(applyingShipObject.transform.position,anchor.transform.position) > 1f)
                            {
                                applyingShipObject.transform.position += (Vector3)(applyingShip.shipData.speed * Time.deltaTime * new Vector2(Mathf.Cos(rad),Mathf.Sin(rad)));
                            }
                        });
                    // var chain = UnityEngine.Object.Instantiate(ChianLineObject);
                    // var LR = chain.GetComponent<LineRenderer>();
                    // LR.positionCount = 2;
                    // LR.SetPosition(0,new Vector3(applyingShipObject.transform.position.x,applyingShipObject.transform.position.y,0f));
                    // LR.SetPosition(1,new Vector3(applyingShipObject.transform.position.x,applyingShipObject.transform.position.y,0f));
                    var interval = Observable.Interval(TimeSpan.FromSeconds(1f))
                        .Do(_=>
                        {
                            if(Vector2.Distance(applyingShipObject.transform.position,anchor.transform.position) <= 1f)
                            {
                                
                            }
                        });
                    return Observable.Merge(everyFrame, interval);
                })
                .Subscribe()
                .AddTo(applyingShipObject);
        }
        public override void Shoot(GameObject applyingShipObject, Ship applyingShip)
        {
            
        }
    }
}

