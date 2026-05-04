using Stats;
using UnityEngine;
using Managers;
using Ships;
using System;
namespace Items
{
    [CreateAssetMenu(menuName = "EventDrivenEffect")]
    [Serializable]
    public class EventDrivenEffect:ItemEffect
    {
        public ShipEventController.EventCategory eventCategory;
        public override void OnApply()
        {
            
        }
        
    }
}

