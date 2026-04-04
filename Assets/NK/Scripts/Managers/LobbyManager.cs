using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance{get;private set;}
        [Header("prefab")]
        public GameObject _DifficultyButtonObject;
        [Header("UI")]
        public Button _StartButton;
        public Button _BackToStartButton;
        public Button _SelectDifficultyMapButton;
        public Button _TechButton;
        public Button _ToMapButton;
        public RectTransform _DifficultyButtonScrollContent;
        [Header("Canvas")]
        public GameObject _LobbyCanvas;
        public GameObject _StartCanvas;
        [Header("Area")]
        public GameObject _DifficultyArea;
        public GameObject _TechTreeArea;
        //
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
            _LobbyCanvas.SetActive(false);
            _StartCanvas.SetActive(true);
            _StartButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _LobbyCanvas.SetActive(true);
                    _StartCanvas.SetActive(false);
                })
                .AddTo(gameObject);
            _BackToStartButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _LobbyCanvas.SetActive(false);
                    _StartCanvas.SetActive(true);
                })
                .AddTo(gameObject);
            _SelectDifficultyMapButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _DifficultyArea.SetActive(true);
                    _TechTreeArea.SetActive(false);
                })
                .AddTo(gameObject);
            _TechButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _DifficultyArea.SetActive(false);
                    _TechTreeArea.SetActive(true);
                })
                .AddTo(gameObject);
            var stageNumDic = new Dictionary<int, int>
            {
                {0,10},
                {1,15},
                {2,20},
                {3,25},
                {4,30}
            }; 
            GManager.Instance._floorNum = stageNumDic[0];
            for(int i = 0;i < stageNumDic.Count;i++)
            {
                var buttonObject = Instantiate(_DifficultyButtonObject);
                buttonObject.transform.SetParent(_DifficultyButtonScrollContent,false);
                var button = buttonObject.GetComponent<Button>();
                int j = i;
                button.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        GManager.Instance._floorNum = stageNumDic[j];
                    })
                    .AddTo(buttonObject);
            }
            _ToMapButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneLoader.Instance.ToMap();
                })
                .AddTo(gameObject);
        }
    }
}

