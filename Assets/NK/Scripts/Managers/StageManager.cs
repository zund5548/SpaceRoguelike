using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance{get;private set;}
        public Button ToStageButton;
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            SetToStageButton();
        }
        private void SetToStageButton()
        {
            if(ToStageButton == null)return;
            ToStageButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneLoader.Instance.ToStage();
                })
                .AddTo(ToStageButton.gameObject);
        }
    }
}

