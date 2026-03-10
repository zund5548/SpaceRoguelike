using UnityEngine;
using System;
using System.Collections.Generic;
namespace Maps
{
    [Serializable]
    public class Star
    {
        public float radius;
        public Sprite sprite;
        public StarType starType;
        [Serializable]
        public enum StarType
        {
            redDwarf,m,k,g,f,a,b,o,neutronStar,blackHole,
        }
    }
}
    
