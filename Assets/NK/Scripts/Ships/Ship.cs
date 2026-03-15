using UnityEngine;
using Ships;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
using Managers;
using Stats;
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

        public Stat currentPower;
        /// <summary>パーセント</summary>
        public Stat shotIntervalReduction;
        public Stat GetStat(StatType type)
        {
            return type switch
            {
                StatType.Power => currentPower,
                StatType.Hull => maxHullPoint,
                StatType.Shield => maxShieldPoint,
                StatType.shotInterval => shotIntervalReduction,
                _ => null
            };
        }
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
            maxShieldPoint = new Stat(shipData.maxShieldPoint); 
            maxHullPoint = new Stat(shipData.maxHullPoint);
            currentPower = new Stat(shipData.power);
            shotIntervalReduction = new Stat(0f);
        }
        
        public void SetCurrent()
        {
            SetCurrentSPHP();
            SetWeapon();
        }
        
        public void SetCurrentSPHP()
        {
            currentShieldPoint = (int)maxShieldPoint.Value;
            currentHullPoint = (int)maxHullPoint.Value;
        }

        public void SetWeapon()
        {
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            foreach(var weapon in shipData.weaponDataList)
            {
                weapon.ShootAction(gameObject,this);
            }
        }

        public void SetShipList(List<GameObject> ally,List<GameObject> opponet)
        {
            allyShipObjectList = ally;
            opponentShipObjectList = opponet;
        }

        public void GetNearestOpponet()
        {
            float mindist = Mathf.Infinity;
            foreach(var shipObject in opponentShipObjectList)
            {
                var dist = Vector2.Distance(transform.position,shipObject.transform.position);
                if(shipObject && mindist > dist)
                {
                    targetObject = shipObject;
                    mindist = dist;
                }
            }
        }
        
        public void DealDamage(Ship dealtShip,int power)
        {
            EventManager.Instance.PublishDamaged(new EventManager.ShipDamageEvent{ship = this,dealingShip = dealtShip,delatDamageValue = power});
            //Debug.Log(currentShieldPoint.ToString()+"/"+currentHullPoint.ToString());
            int actualPower = power;
            if(currentShieldPoint > 0)
            {
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

            }
            if(actualPower <= 0)return;
            if(currentHullPoint > 0)
            {
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
                Destroy(gameObject);
            }
        }
    }
}


