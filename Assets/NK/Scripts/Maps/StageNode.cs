using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    public class StageNode
    {
        public string stageName;
        public int floorStageNum;
        public Vector2 buttonLocalPos;
        public StageType stageType;
        public GameObject buttonObject;
        [Serializable]
        public enum StageType
        {
            lobby,
            battle,
            defence,
            credit,
            boss
        }
        public List<StageNode> nextNodeList = new();
    }
}
