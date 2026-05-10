using UnityEngine;
using Ships;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
using Managers;
using Stats;
using System;
using Items;
namespace Ships
{
    public class Ship : MonoBehaviour
    {
        public bool isPlayer = true;
        public bool isSurged = false;//trueで移動不可
        public ShipData shipData;
        public int currentShieldPoint;
        public int currentHullPoint;

        //public GameObject targetObject;
        public List<GameObject> allyShipObjectList = new List<GameObject>();
        public List<GameObject> opponentShipObjectList = new List<GameObject>();

        public Stat maxShieldPoint{get; private set;}
        public Stat maxHullPoint{get; private set;}
        public Stat currentPower;
        public Stat critRate;
        /// <summary>砲撃間隔の減少(%)</summary>
        public Stat shotIntervalReduction;
        public Stat shieldResistance;
        public Stat hullResistance;
        public UniqueStatController uniqueStatController = new();
        public ShipEventController shipEventController = new();
        public Stat GetStat(StatType type)
        {
            return type switch
            {
                StatType.Power => currentPower,
                StatType.CritRate => critRate,
                StatType.Shield => maxShieldPoint,
                StatType.Hull => maxHullPoint,
                StatType.ShieldResistance => shieldResistance,
                StatType.HullResistance => hullResistance,
                StatType.ShotIntervalReduction => shotIntervalReduction,
                _ => null
            };
        }
        //スタック
        public StackEffectController stackEffectController = new();
        //
        public void Start()
        {
            if(!isPlayer)return;
            gameObject.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    int n = allyShipObjectList.Count;
                    for(int i = 0;i < n;i++)
                    {
                        if(allyShipObjectList[i] == gameObject)
                        {
                            allyShipObjectList.RemoveAt(i);
                            break;
                        }
                    }
                })
                .AddTo(gameObject);
        }
        public void SetMaxSPHP()
        {
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            maxShieldPoint = new Stat(shipData.maxShieldPoint); 
            maxHullPoint = new Stat(shipData.maxHullPoint);
        }
        public void SetStats()
        {
            
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            currentPower = new Stat(shipData.power);
            critRate = new Stat(shipData.critRate);
            maxShieldPoint = new Stat(shipData.maxShieldPoint); 
            maxHullPoint = new Stat(shipData.maxHullPoint);
            shieldResistance = new Stat(shipData.shieldResistance);
            hullResistance = new Stat(shipData.hullResistance);
            currentPower = new Stat(shipData.power);
            shotIntervalReduction = new Stat(0f);
            //shotSpeed = new Stat(shipData.shotSpeed);
        }
        
        // public void SetCurrent()
        // {
        //     SetCurrentSPHP();
        //     SetCurrentUniqueStat();
        //     SetCurrentWeapon();
        // }
        
        public void SetCurrentSPHP()
        {
            //Debug.Log((int)maxShieldPoint.Value);
            currentShieldPoint = (int)maxShieldPoint.Value;
            currentHullPoint = (int)maxHullPoint.Value;
        }

        public void SetCurrentUniqueStat()
        {
        
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            if(shipData.weaponData != null)
            {
                shipData.weaponData.SetUniqueStat(this);
            }
        }

        public void SetCurrentWeapon()
        {
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            if(shipData.weaponData != null)
            {
                shipData.weaponData.ShootAction(gameObject,this);
            }
            
        }

        public void SetShipList(List<GameObject> ally,List<GameObject> opponet)
        {
            allyShipObjectList = ally;
            opponentShipObjectList = opponet;
        }

        public GameObject GetNearestOpponet()
        {
            float mindist = Mathf.Infinity;
            GameObject result = null;
            foreach(var shipObject in opponentShipObjectList)
            {
                var dist = Vector2.Distance(transform.position,shipObject.transform.position);
                if(shipObject && mindist > dist)
                {
                    result = shipObject;
                    mindist = dist;
                }
            }
            return result;
        }
        /// <summary>このshipにダメージを与える</summary>
        /// <param name="power">ダメージ量</param>
        /// <param name="dealerShip">ダメージを与えた船</param>
        public void DealDamage(int power,bool isCritEnable,Ship dealerShip = null)
        {
            int actualPowerSum = 0;
            
            bool isShieldDamaged = false;
            //Debug.Log(currentShieldPoint.ToString()+"/"+currentHullPoint.ToString());
            int actualPower = power;
            if(currentShieldPoint > 0)
            {
                //actualPower = (int)(actualPower  - shieldResistance.Value) > 0 ? (int)(actualPower - shieldResistance.Value) : 0;
                actualPower = (int)(actualPower * (100f - shieldResistance.Value)/100f) > 0 ?  (int)(actualPower * (100f - shieldResistance.Value)/100f):0;
                actualPowerSum += actualPower;
                if(currentShieldPoint > actualPower)
                {
                    ShipManager.Instance.SetDamagevalue(actualPower,transform.position,true,isCritEnable);
                    currentShieldPoint -= actualPower;
                    actualPower = 0;
                }
                else
                {
                    ShipManager.Instance.SetDamagevalue(currentShieldPoint,transform.position,true,isCritEnable);
                    actualPower -= currentShieldPoint;
                    currentShieldPoint = 0;
                }
                isShieldDamaged = true;
            }
           
            if(currentHullPoint > 0)
            {
                //actualPower = (int)(actualPower - hullResistance.Value) > 0 ? (int)(actualPower - hullResistance.Value) : 0;
                actualPower = (int)(actualPower * (100f - hullResistance.Value)/100f) > 0 ?  (int)(actualPower * (100f - hullResistance.Value)/100f):0;
                actualPowerSum += actualPower;
                if(isShieldDamaged && actualPower <= 0)
                {
                    SetDamagingEvent(dealerShip,actualPowerSum);
                    return;
                }
                if(currentHullPoint > actualPower)
                {
                    ShipManager.Instance.SetDamagevalue(actualPower,transform.position,false,isCritEnable);
                    currentHullPoint -= actualPower;
                    actualPower = 0;
                }
                else
                {
                    ShipManager.Instance.SetDamagevalue(currentHullPoint,transform.position,false,isCritEnable);
                    actualPower -= currentHullPoint;
                    currentHullPoint = 0;
                }
            }
            SetDamagingEvent(dealerShip,actualPowerSum);
            if(currentHullPoint == 0)
            {
                if(!isPlayer)GManager.Instance.AddCredit(shipData.shipType);
                Kill(dealerShip);
            }
            
        }
        private void SetDamagingEvent(Ship dealerShip,int power)
        {
            if(dealerShip)dealerShip.shipEventController.PublishDamaging(new ShipEventController.ShipAttackEvent
            {
                targetShip = this,
                dealerShip = dealerShip,
                dealtDamageValue = power
            });
        }
        public void Kill(Ship killerShip)
        {
            int n = allyShipObjectList.Count;
            for(int i = 0;i < n;i++)
            {
                if(allyShipObjectList[i] == gameObject)
                {
                    allyShipObjectList.RemoveAt(i);
                    break;
                }
            }
            if(isPlayer && allyShipObjectList.Count == 0)EventManager.Instance.PublishFail();
            killerShip.shipEventController.PublishKilling(new ShipEventController.ShipAttackEvent(this,killerShip));
            Destroy(gameObject);
        }
    }
}


