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
    [CreateAssetMenu(menuName = "StageEncount/ObtainShipEncount")]
    [Serializable]
    public class ObtainShipEncount:StageEncount
    {
        public override IEnumerator SetStageEncount()
        {
            EventManager.OnStageClear
            .Subscribe(_ =>
            {
                StageManager.Instance.StageClear();
            })
            .AddTo(EventManager.Instance);
            yield return ObtainShipCoroutine();
            EventManager.Instance.PublishClear();
        }
        public IEnumerator ObtainShipCoroutine()
        {
            ShipManager.Instance.DeleteAllPlayer();
            yield return StageManager.Instance.SetShipDataCoroutine();
        }
    }
}