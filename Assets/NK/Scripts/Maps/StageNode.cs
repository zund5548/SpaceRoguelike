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
            lobby,
            battle,
            defence,
            credit,
            elite,
            boss
        }
        [SerializeReference]
        public List<StageNode> nextNodeList = new();
        public List<Planet> planetList = new();
        public List<Star> starList = new();
        public StageEncount stageEncount;
    }
}
