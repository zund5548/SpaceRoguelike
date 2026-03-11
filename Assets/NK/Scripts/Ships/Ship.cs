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
        public int currentShieldPoint{get; private set;} 
        public int maxShieldPoint{get; private set;}
        public int currentHullPoint{get; private set;}
        public int maxHullPoint{get; private set;}

        public GameObject targetObject;
        public List<GameObject> allyShipObjectList = new List<GameObject>();
        public List<GameObject> opponentShipObjectList = new List<GameObject>();

        public Stat currentPower;
        public void Start()
        {
            // gameObject.UpdateAsObservable()
            //     .Subscribe(_ =>
            //     {
            //         GetNearestEnemy();
            //     })
            //     .AddTo(gameObject);
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
        public void SetSPHP()
        {
            if(shipData == null)
            {
                Debug.Log("shipData is null");
                return;
            }
            maxShieldPoint = shipData.maxShieldPoint;
            currentShieldPoint = maxShieldPoint;
            maxHullPoint = shipData.maxHullPoint;
            currentHullPoint = maxHullPoint;
        }
        public void SetShipList(List<GameObject> ally,List<GameObject> opponet)
        {
            allyShipObjectList = ally;
            opponentShipObjectList = opponet;
        }
        public void GetNearestEnemy()
        {
            float mindist = Mathf.Infinity;
            foreach(var shipObject in opponentShipObjectList)
            {
                if(shipObject && mindist > Vector2.Distance(transform.position,shipObject.transform.position))
                {
                    targetObject = shipObject;
                }
            }
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
        public virtual void DealDamage(int power)
        {
            //Debug.Log(currentShieldPoint.ToString()+"/"+currentHullPoint.ToString());
            int actualPower = power;
            if(currentShieldPoint > 0)
            {
                ShipManager.Instance.SetDamagevalue(actualPower,transform.position,true);
                if(currentShieldPoint > actualPower)
                {
                    currentShieldPoint -= actualPower;
                    actualPower = 0;
                }
                else 
                {
                    actualPower -= currentShieldPoint;
                    currentShieldPoint = 0;
                }
                
            }
            if(actualPower == 0)return;
            if(currentHullPoint > 0)
            {
                ShipManager.Instance.SetDamagevalue(actualPower,transform.position,false);
                if(currentHullPoint > actualPower)
                {
                    currentHullPoint -= actualPower;
                }
                else 
                {
                    //truePower -= currentHullPoint;
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


