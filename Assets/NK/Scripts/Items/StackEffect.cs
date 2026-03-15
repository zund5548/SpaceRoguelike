using Ships;
using UnityEngine;

namespace Items
{
    public abstract class StackEffect
    {
        public Ship ownerShip;
        public int stackNum = 0;
        public int threshold = 5;
        public bool isAbletoAdd = true;
        public void AddStack(int value)
        {
            if(!isAbletoAdd)return;
            stackNum += value;
        }
        public abstract void OnStackChanged();
    }
}

