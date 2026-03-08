using UnityEngine;
using Ships;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
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
        public void Start()
        {
            gameObject.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    GetNearestEnemy();
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
        public void SetAO(List<GameObject> ally,List<GameObject> opponet)
        {
            allyShipObjectList = ally;
            opponentShipObjectList = opponet;
        }
        public void GetNearestEnemy()
        {
            float mindist = Mathf.Infinity;
            foreach(var shipObject in opponentShipObjectList)
            {
                if(mindist > Vector2.Distance(transform.position,shipObject.transform.position))
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
            GetNearestEnemy();
            foreach(var weapon in shipData.weaponDataList)
            {
                weapon.ShootAction(gameObject,targetObject.transform.position);
            }
        }
        public virtual void DealDamage(int power)
        {
            int truePower = power;
            if(currentShieldPoint > 0)
            {
                if(currentShieldPoint > truePower)
                {
                    currentShieldPoint -= truePower;
                }
                else 
                {
                    truePower -= currentShieldPoint;
                    currentShieldPoint = 0;
                }
            }
            if(currentHullPoint > 0)
            {
                if(currentHullPoint > truePower)
                {
                    currentHullPoint -= truePower;
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


