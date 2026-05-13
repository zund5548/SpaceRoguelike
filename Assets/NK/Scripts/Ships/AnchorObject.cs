using Stats;
using UnityEngine;
namespace Ships
{
    public class AnchorObject : Ship
    {
        public int maxAnchorHullPoint = 5;
         public int currentAnchorHullPoint = 0;
        public new void Start()
        {
            currentAnchorHullPoint =  maxAnchorHullPoint;
        }
        public override void DealDamage(int power,bool isCritable,Ship dealerShip = null)
        {
            currentAnchorHullPoint -= power;
            if(currentHullPoint <= 0)Kill(dealerShip);
        }
    }
}

