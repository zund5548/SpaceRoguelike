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
            var anchorObjectComp = anchor.GetComponent<AnchorObject>();
            
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
            bool isReached = false;
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
                    
                    var interval = Observable.Interval(TimeSpan.FromSeconds(0.1f))
                        .Do(_=>
                        {
                            if(Vector2.Distance(applyingShipObject.transform.position,anchor.transform.position) <= 1f)
                            {
                                if(!isReached)
                                {
                                    isReached = true;
                                    SetLaser(applyingShipObject,applyingShipObject.transform.position,anchor.transform.position);
                                }
                                if(anchorObjectComp)anchorObjectComp.DealDamage((int)applyingShip.currentPower.Value,false,applyingShip);
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
        private void SetLaser(GameObject applyingShipObject,Vector2 s,Vector2 e)
        {
            //linerendererをlaserとする
            var laser = UnityEngine.Object.Instantiate(ChianLineObject);
            var LR = laser.GetComponent<LineRenderer>();
            Gradient gradient = new();
            if(applyingShipObject.CompareTag("PlayerShip"))
            {
                gradient.SetKeys
                (
                    new GradientColorKey[]
                    {
                        new GradientColorKey(Color.turquoise, 0f),
                        new GradientColorKey(Color.turquoise, 1f)
                    },
                    new GradientAlphaKey[]
                    {
                        //new GradientAlphaKey(0.5f, 0f),
                        new GradientAlphaKey(1f, 1f)
                    }
                );
            }
            else
            {
                gradient.SetKeys
                (
                    new GradientColorKey[]
                    {
                        new GradientColorKey(Color.orange, 0f),
                        new GradientColorKey(Color.orange, 1f)
                    },
                    new GradientAlphaKey[]
                    {
                        // new GradientAlphaKey(0.5f, 1f),
                        new GradientAlphaKey(1f, 1f)
                    }
                );
            }
            LR.colorGradient = gradient;
            LR.positionCount = 2;
            LR.SetPosition(0,new Vector3(s.x,s.y,0f));
            LR.SetPosition(1,new Vector3(e.x,e.y,0f));
            applyingShipObject.OnDestroyAsObservable()
                .Subscribe(_=>UnityEngine.Object.Destroy(laser));
        }
    }
}

