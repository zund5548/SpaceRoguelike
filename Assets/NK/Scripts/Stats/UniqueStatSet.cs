using System;
using Items;
using UnityEngine;
namespace Stats
{
    [Serializable]
    public class UniqueStatSet
    {
        public virtual Stat GetStat()
        {
            return new Stat(0);
        }
    }
}

