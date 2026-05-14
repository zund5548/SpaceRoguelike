using Ships;
using UnityEngine;

namespace Items
{
    //しきい値を超えるとownerShipに処理をする
    public class StackEffect
    {
        [HideInInspector]
        public Ship dealerShip;
        [HideInInspector]
        public Ship ownerShip;
        [HideInInspector]
        public int stackNum = 0;
        [HideInInspector]
        public float possibility = 100;
        [HideInInspector]
        public bool isAbletoAdd = true;
        public void AddStack(int value)
        {
            if(!isAbletoAdd)return;
            if(UnityEngine.Random.Range(1,1000) > possibility * 10)
            {
                Debug.Log("add stack:miss");
                return;
            }
            stackNum += value;
        }
        public virtual void OnStackChanged(){}
    }
}

