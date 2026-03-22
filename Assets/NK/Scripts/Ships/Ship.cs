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
        public ShipData shipData;
        public int currentShieldPoint;
        public Stat maxShieldPoint{get; private set;}
        public int currentHullPoint;
        public Stat maxHullPoint{get; private set;}

        public GameObject targetObject;
        public List<GameObject> allyShipObjectList = new List<GameObject>();
        public List<GameObject> opponentShipObjectList = new List<GameObject>();

        public bool isAbleToMove = true;//falseで移動不可
        public Stat currentPower;
        /// <summary>単位:パーセント</summary>
        public Stat shotIntervalReduction;
        public Stat shieldResistance;
        public Stat hullResistance;
        /// <summary>一度に発射する弾の数</summary>
        public Stat projectileNum;
        public Stat GetStat(StatType type)
        {
            return type switch
            {
                StatType.Power => currentPower,
                StatType.Shield => maxShieldPoint,
                StatType.Hull => maxHullPoint,
                StatType.ShieldResistance => shieldResistance,
                StatType.HullResistance => hullResistance,
                StatType.ProjectileNum => projectileNum,
                StatType.ShotInterval => shotIntervalReduction,
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
        // public void SetStat()
        // {
        //     SetSPHP();
        //     SetWeapon();
        // }
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
            maxShieldPoint = new Stat(shipData.maxShieldPoint); 
            maxHullPoint = new Stat(shipData.maxHullPoint);
            shieldResistance = new Stat(shipData.shieldResistance);
            hullResistance = new Stat(shipData.hullResistance);
            currentPower = new Stat(shipData.power);
            projectileNum = new Stat(shipData.projectileNum);
            shotIntervalReduction = new Stat(0f);
        }
        
        public void SetCurrent()
        {
            SetCurrentSPHP();
            SetCurrentWeapon();
        }
        
        public void SetCurrentSPHP()
        {
            currentShieldPoint = (int)maxShieldPoint.Value;
            currentHullPoint = (int)maxHullPoint.Value;
        }

        public void SetCurrentWeapon()
        {
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            foreach(var weapon in shipData.weaponDataList)
            {
                Debug.Log("a");
                weapon.ShootAction(gameObject,this);
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
            targetObject = result;
            return result;
        }
        /// <summary>このshipにダメージを与える</summary>
        /// <param name="power">ダメージ量</param>
        /// <param name="dealtShip">ダメージを与えた船</param>
        public void DealDamage(int power,Ship dealtShip = null)
        {
            bool isShieldDamaged = false;
            if(dealtShip != null)EventManager.Instance.PublishDamaged(new EventManager.ShipDamageEvent{targetShip = this,dealingShip = dealtShip,delatDamageValue = power});
            //Debug.Log(currentShieldPoint.ToString()+"/"+currentHullPoint.ToString());
            int actualPower = power;
            if(currentShieldPoint > 0)
            {
                actualPower = (int)(actualPower  - shieldResistance.Value) > 0 ? (int)(actualPower - shieldResistance.Value) : 0;
                if(currentShieldPoint > actualPower)
                {
                    ShipManager.Instance.SetDamagevalue(actualPower,transform.position,true);
                    currentShieldPoint -= actualPower;
                    actualPower = 0;
                }
                else
                {
                    ShipManager.Instance.SetDamagevalue(currentShieldPoint,transform.position,true);
                    actualPower -= currentShieldPoint;
                    currentShieldPoint = 0;
                }
                isShieldDamaged = true;
            }
            if(currentHullPoint > 0)
            {
                actualPower = (int)(actualPower - hullResistance.Value) > 0 ? (int)(actualPower - hullResistance.Value) : 0;
                if(isShieldDamaged && actualPower <= 0)return;
                if(currentHullPoint > actualPower)
                {
                    ShipManager.Instance.SetDamagevalue(actualPower,transform.position,false);
                    currentHullPoint -= actualPower;
                    actualPower = 0;
                }
                else
                {
                    ShipManager.Instance.SetDamagevalue(currentHullPoint,transform.position,false);
                    actualPower -= currentHullPoint;
                    currentHullPoint = 0;
                }
            }

            if(currentHullPoint == 0)
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
                if(isPlayer && allyShipObjectList.Count == 0)EventManager.Instance.PublishFalse();
                Destroy(gameObject);
            }
        }
    }
}


