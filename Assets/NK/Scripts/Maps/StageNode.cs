using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    [Serializable]
    public class StageNode
    {
        public string stageName;
        public int floorStageNum;
        public Vector2 buttonLocalPos;
        public StageType stageType;
        public (int,int) mapIdx;//マップでのインデックス
        //public GameObject buttonObject;
        [Serializable]
        public enum StageType
        {
            Lobby,
            Battle,
            Defence,
            Shop,
            Credit,
            ObtainShip,
            Elite,
            Boss,
        }
        [SerializeReference]
        public List<StageNode> nextNodeList = new();
        public List<Star> starList = new();
        public List<Planet> planetList = new();
        public StageEncount stageEncount;

        public string GetStageTypeStr()
        {
            //if((int)stageType == 7)Debug.Log("stageType isn't set");
            return stageType switch
            {
                StageType.Lobby => "Lobby",
                StageType.Battle => "Combat",
                StageType.Defence => "Defence",
                StageType.Shop => "Shop",
                StageType.Credit => "Bounty",
                StageType.ObtainShip => "Add Ship",
                StageType.Elite => "Elite",
                StageType.Boss => "Boss",
                _=> ""
            };
        }
    }
}
