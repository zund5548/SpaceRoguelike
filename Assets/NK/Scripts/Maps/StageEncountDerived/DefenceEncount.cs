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
        public ShipData anchorData;
        public override IEnumerator SetStageEncount()
        {
            EventManager.OnStageClear
            .Subscribe(_ =>
            {
                StageManager.Instance.StageClear();
            })
            .AddTo(EventManager.Instance);
            yield return DefenceCoroutine();
        }
        public IEnumerator DefenceCoroutine()
        {
            ShipManager.Instance.SpawnAnchor(anchorData);
            yield return null;
        }
    }
}