using UnityEngine;
using System;
using System.Collections.Generic;
namespace Maps
{
    [CreateAssetMenu(fileName = "StarDatabase", menuName = "Database/StarDatabase")]
    [Serializable]
    public class StarDatabase : ScriptableObject
    {
        public List<Star> starList = new();
        public Star GetRandomStar()
        {
            int count = 0;
            Star result = null;
            foreach (var planet in starList)
            {
                if (UnityEngine.Random.Range(0, count) == 0)
                {
                    result = planet;
                }
                count++;
            }
            return result;
        }
    }
}

