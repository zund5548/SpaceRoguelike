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
    [CreateAssetMenu(menuName = "StageEncount/BossEncount")]
    [Serializable]
    public class BossEncounter : StageEncount
    {
        [SerializeReference,SubclassSelector]
        public BossType bossType;
        public ShipData shipData;
        public override IEnumerator SetStageEncount()
        {
            EventManager.OnBossBeat
                .Subscribe(_ =>
                {
                    Debug.Log("Clear");
                    StageManager.Instance.BossBeat();
                })
                .AddTo(EventManager.Instance);
            yield return ShipManager.Instance.BossEncountCoroutine(shipData,bossType);
            EventManager.Instance.PublishBossBeat();
        }
    }
}

