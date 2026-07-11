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
            RedDwarf,M,K,G,F,A,B,O,NeutronStar,BlackHole,
        }
        private static Dictionary<StarType,string> starColorDic = new Dictionary<StarType, string>
        {
            {StarType.RedDwarf,"#ff0000"},
            {StarType.M,"#ff0000"},
            {StarType.K,"#ff8000"},
            {StarType.G,"#fbff00"},
            {StarType.F,"#fbff00"},
            {StarType.A,"#feffe1"},
            {StarType.B,"#76faf6"},
            {StarType.O,"#16e1fc"},
            {StarType.NeutronStar,"#ffffff"},
            {StarType.BlackHole,"#000000"},
        };
        public Color GetColor()
        {
            Color color = Color.black;
            ColorUtility.TryParseHtmlString(starColorDic[starType], out color);
            return color;
        }
    }
}
    
