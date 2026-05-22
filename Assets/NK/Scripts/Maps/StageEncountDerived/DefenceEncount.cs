using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Maps;
using Managers;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Ships;

namespace Maps
{
    [CreateAssetMenu(menuName = "StageEncount/DefenceEncount")]
    [Serializable]
    public class DefenceEncount:StageEncount
    {
        public int oneWaveLimit;//ウェーブ内で１度に出てくる敵の最大
        public List<EnemyWave> enemyWaveList = new();
        public ShipData anchorData;
        public override IEnumerator SetStageEncount()
        {
             var disposable =EventManager.OnStageFail
                .Subscribe(_ =>
                {
                    StageManager.Instance.GameFail();
                })
                .AddTo(EventManager.Instance);
                
            EventManager.OnStageClear
            .Subscribe(_ =>
            {
                StageManager.Instance.StageClear();
            })
            .AddTo(EventManager.Instance);

            var anchorObject = ShipManager.Instance.SpawnAnchor(anchorData);
            anchorObject.OnDestroyAsObservable()
            .Subscribe(_=>
            {
                EventManager.Instance.PublishFail();
            });
            yield return ShipManager.Instance.SetBattleEncountWave(enemyWaveList,oneWaveLimit);
            disposable.Dispose();
            EventManager.Instance.PublishClear();
        }
    }
}