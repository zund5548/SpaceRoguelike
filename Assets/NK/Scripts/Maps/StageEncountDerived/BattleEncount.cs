using System;
using System.Collections.Generic;
using Maps;
using Ships;
using Managers;
using UnityEngine;
namespace Maps
{
    [Serializable]
    public class BattleEncount : StageEncount
    {
        public int oneWaveLimit;//ウェーブ内で１度に出てくる敵の最大
        public List<EnemyWave> enemyWaveList = new();
        public override void SetStageEncount()
        {
            if(ShipManager.Instance)ShipManager.Instance.BattleEncountWave(enemyWaveList,oneWaveLimit);
        }
    }
    [Serializable]
    public class EnemyWave
    {
        public List<ShipData> shipDataList = new();
    }
}

