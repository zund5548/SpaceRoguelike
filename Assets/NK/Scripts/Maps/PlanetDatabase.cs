using UnityEngine;
using System;
using System.Collections.Generic;
namespace Maps
{
    [CreateAssetMenu(fileName = "PlanetDatabase", menuName = "Database/PlanetDatabase")]
    [Serializable]
    public class PlanetDatabase : ScriptableObject
    {
        public List<Planet> planetList = new();
        public Planet GetRandomPlanet()
        {
            int count = 0;
            Planet result = null;
            foreach (var planet in planetList)
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

