using System;
using System.Collections.Generic;
using Maps;
using Ships;
using Managers;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
namespace Maps
{
    [CreateAssetMenu(menuName = "StageEncount/StageEncount")]
    [Serializable]
    public class BattleEncount : StageEncount
    {
        public int oneWaveLimit;//ウェーブ内で１度に出てくる敵の最大
        public List<EnemyWave> enemyWaveList = new();
        public override IEnumerator SetStageEncount()
        {
            EventManager.Instance.FailDisposable = EventManager.OnStageFail
                .Subscribe(_ =>
                {
                    StageManager.Instance.GameFail();
                })
                .AddTo(EventManager.Instance);
            EventManager.Instance.ClearDisposable = EventManager.OnStageClear
                .Subscribe(_ =>
                {
                    StageManager.Instance.StageClear();
                })
                .AddTo(EventManager.Instance);
            yield return ShipManager.Instance.BattleEncountWave(enemyWaveList,oneWaveLimit);
            EventManager.Instance.PublishClear();
        }
    }
    [Serializable]
    public class EnemyWave
    {
        public List<ShipData> shipDataList = new();
    }
}

