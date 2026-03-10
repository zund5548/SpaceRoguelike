using UnityEngine;
using System;
using System.Collections.Generic;
namespace Maps
{
    [Serializable]
    public class Planet
    {   
        public float radius;
        public Sprite sprite;
        public PlanetType planetType;
        [Serializable]
        public enum PlanetType
        {
            rocky,gaseous,ring
        }
    }
}
   
