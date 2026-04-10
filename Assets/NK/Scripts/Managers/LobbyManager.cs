using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
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
        Dictionary<int, int> stageNumDic = new Dictionary<int, int>
        {
            {0,10},
            {1,15},
            {2,20},
            {3,25},
            {4,30}
        }; 
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
            for(int i = 0;i < stageNumDic.Count;i++)
            {
                var buttonObject = Instantiate(_DifficultyButtonObject);
                buttonObject.transform.SetParent(_DifficultyButtonScrollContent,false);
                //文字設定
                var buttonText = buttonObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text =  "Level " + (i + 1).ToString() + "\n" + stageNumDic[i].ToString() + " Floors";
                //
                var button = buttonObject.transform.GetChild(0).GetComponent<Button>();
                int j = i;
                button.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        SetDifficulty(j);
                    })
                    .AddTo(buttonObject);
            }
            SetDifficulty(0);
            _ToMapButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneLoader.Instance.ToMap();
                })
                .AddTo(gameObject);
        }
        //表示する難易度はlevel+1
        public void SetDifficulty(int level)
        {
            if(level >= stageNumDic.Count)level = stageNumDic.Count - 1;
            foreach(Transform buttonObject in _DifficultyButtonScrollContent.transform)
            {
                var image = buttonObject.GetComponent<Image>();
                var c = image.color;
                c.a = 0f;
                image.color = c;
            }
            var buttonImage = _DifficultyButtonScrollContent.transform.GetChild(level).GetComponent<Image>();
            var newColor = buttonImage.color;
            newColor.a = 1f;
            buttonImage.color = newColor;
            GManager.Instance._floorNum = stageNumDic[level];
        }
    }
}

