using System;
using System.Collections.Generic;
using Maps;
using Ships;
using Managers;
using UnityEngine;
using System.Collections;
namespace Maps
{
    [Serializable]
    public class StageEncount:ScriptableObject
    {
        public StageNode.StageType stageType;
        public Sprite icon;
        /// <summary>
        /// -1:難易度を必要としない
        /// </summary>
        public int dificulty;
        public virtual IEnumerator SetStageEncount()
        {
            yield return null;
        }
    }
}

