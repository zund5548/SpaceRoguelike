using Managers;
using Stats;
using UnityEngine;
namespace Ships
{
    public class AnchorObject : Ship
    {
        public int maxAnchorHullPoint;
        public int currentAnchorHullPoint;
        public new void Start()
        {
            maxAnchorHullPoint = shipData.maxHullPoint;
            currentAnchorHullPoint =  maxAnchorHullPoint;
            StageManager.Instance._AnchorGauge.maxValue = maxAnchorHullPoint;
            StageManager.Instance._AnchorGauge.value = maxAnchorHullPoint;
            //StageManager.Instance._AnchorGauge.gameObject.SetActive(true);
        }
        public override void DealDamage(int power,bool isCritable,Ship dealerShip = null)
        {
            currentAnchorHullPoint -= power;
            StageManager.Instance._AnchorGauge.value = currentAnchorHullPoint;
            if(currentAnchorHullPoint <= 0)Kill(dealerShip);
        }
    }
}

