using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Maps;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using Unity.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UIElements;
namespace Managers
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance{get;private set;}
        public int _floorNum;
        public float scrollVerticalSize;
        public float stageButtonSize;
        [Header("UI")]
        public UnityEngine.UI.Button _toStageButton;
        public ScrollRect _mapScrollRect;
        public RectTransform _mapScrollContent;
        public Canvas MapCanvas;
        public TextMeshProUGUI stageNameDisplay;
        [Header("prefab")]
        public GameObject FloorObject;
        public GameObject StageButtonObject;
        public GameObject MapLineObject;
        [Header("database")]
        public PlanetDatabase planetDatabase;
        public StarDatabase starDatabase;
        //
        //public int p,q;
        public List<List<StageNode>> _stageFloorList = new();
        List<List<GameObject>> _stageButtonObjectFloorList = new();
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
            _toStageButton.interactable = false;
            SetToStageButton();
            if(!GManager.Instance.isMapCreated)CreateMap(_floorNum,4,6);
            InstantiateMap();
            SetStageDescription(_stageFloorList[0][0]);
            EnableStageButton();
            _mapScrollRect.verticalNormalizedPosition = 1f/ _floorNum * GManager.Instance.currentStageNode.floorStageNum;
        }
        void Update()
        {
            //debug
            // if(Input.GetKeyDown(KeyCode.Q))
            // {
            //     Debug.Log(_stageNodes[p][q].stageName);
            // }
        }
        private void SetToStageButton()
        {
            if(_toStageButton == null)return;
            _toStageButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneLoader.Instance.ToStage();
                    _toStageButton.interactable = false;
                })
                .AddTo(_toStageButton.gameObject);
        }
        string GetRandomStageName(int n,int m)
        {
            string result = "";
            for(int i = 0;i < n;i++)
            {
                result += (char)('A' + UnityEngine.Random.Range(0,26));
            }
            result += "-";
            for(int i = 0;i < m;i++)
            {
                result += UnityEngine.Random.Range(0,10).ToString();
            }
            return result;
        }
        private List<Star> GetRandomStars(int n)
        {
            var stars = new List<Star>();
            for(int i = 0;i < n;i++)
            {
                stars.Add(starDatabase.GetRandomStar());
            }
            return stars;
        }
        private List<Planet> GetRandomPlanet(int n)
        {
            var planets = new List<Planet>();
            for(int i = 0;i < n;i++)
            {
                planets.Add(planetDatabase.GetRandomPlanet());
            }
            return planets;
        }
        /// <summary>mapランダム生成</summary>
        void CreateMap(int floorNum,int minStageNum,int maxStageNum)
        {
            if(GManager.Instance.isMapCreated)return;
            else GManager.Instance.isMapCreated = true;
            //lobby
            var floorStages = new List<StageNode>();
            var stage = new StageNode
            {
                stageName = "lobby",
                floorStageNum = 0,
                buttonLocalPos = new Vector2(0f,0f),
                stageType = StageNode.StageType.battle,
            };
            floorStages.Add(stage);
            _stageFloorList.Add(floorStages);
            GManager.Instance.currentStageNode = stage;

            float horizontalSize = 1000f;
            //normal stage
            for(int i = 0;i < floorNum;i++)
            {
                int stageNum = UnityEngine.Random.Range(minStageNum,maxStageNum+1);
                float left = -horizontalSize/2f,right = left + horizontalSize/stageNum;//ボタンを置く位置のx座標の上限・下限
                floorStages = new List<StageNode>();
                for(int j = 0;j < stageNum;j++)
                {
                    stage = new StageNode
                    {
                        stageName = GetRandomStageName(2,4),
                        floorStageNum = i + 1,
                        buttonLocalPos = new Vector2(UnityEngine.Random.Range(left+30f,right-30f),UnityEngine.Random.Range(-50f,50f)),
                        stageType = StageNode.StageType.battle,
                        starList = GetRandomStars(3),
                        planetList = GetRandomPlanet(3)
                        
                    };
                    floorStages.Add(stage);
                    left += horizontalSize/stageNum;
                    right += horizontalSize/stageNum;
                }
                _stageFloorList.Add(floorStages);
            }
            //boss
            floorStages = new List<StageNode>();
            stage = new StageNode
            {
                stageName = "??-????",
                floorStageNum = floorNum + 1,
                buttonLocalPos = new Vector2(0f,0f),
                stageType = StageNode.StageType.boss,
            };
            floorStages.Add(stage);
            _stageFloorList.Add(floorStages);
            //stageの連結
            //ランダムウォーク
            int pathNum = 10;
            for(int i = 0;i < pathNum;i++)
            {
                //int idx = i < _stageNodeList[0].Count?i:Random.Range(0,_stageNodeList[0].Count);
                int idx = 0;
                for(int j = 0;j < _stageFloorList.Count-1;j++)
                {
                    bool isSame = false;
                    int nextIdx;
                    if(j == 0)nextIdx = i < _stageFloorList[1].Count?i:_stageFloorList[1].Count-1;
                    else nextIdx =  Mathf.Clamp(idx + UnityEngine.Random.Range(-1, 2),0,_stageFloorList[j+1].Count - 1);
                    //nextIdx =  Mathf.Clamp(idx + UnityEngine.Random.Range(-1, 2),0,_stageFloorList[j+1].Count - 1);
                    foreach(var node in  _stageFloorList[j][idx].nextNodeList)
                    {
                        if(node == _stageFloorList[j+1][nextIdx])isSame = true;
                    }
                    if(!isSame)_stageFloorList[j][idx].nextNodeList.Add(_stageFloorList[j+1][nextIdx]);
                    //Debug.DrawRay(_stageNodeList[j][idx].localPos,_stageNodeList[j+1][nextIdx].localPos,Color.red,20f);
                    idx = nextIdx;
                }
            }
            // for(int i = 0;i < _stageFloorList.Count-1;i++)
            // {
            //     for(int j = 0;j < _stageFloorList[i].Count-1;j++)
            //     {
            //         if(_stageFloorList[i][j].nextNodeList.Count == 0)
            //         Destroy(_stageFloorList[i][j].buttonObject);
            //     }
            // }
            // 接続していないノードを削除
            for(int i = 0;i < _stageFloorList.Count-1;i++)
            {
                _stageFloorList[i].RemoveAll(n => n.nextNodeList.Count == 0);
            }
            //mapIdx代入
            int p = 0;
            foreach(var floor in _stageFloorList)
            {
                int q = 0;
                foreach(var stageNode in floor)
                {
                    stageNode.mapIdx = (p,q);
                    q++;
                }
                p++;
            }
            GManager.Instance._stageFloorList = _stageFloorList;
        }
        /// <summary>mapを元にボタンと線を生成</summary>
        void InstantiateMap()
        {
            if(GManager.Instance.isMapCreated)_stageFloorList = GManager.Instance._stageFloorList;
            for(int i = 0;i < _stageFloorList.Count;i++)
            {
                var floor = Instantiate(FloorObject);
                floor.transform.SetParent(_mapScrollContent,false);
                var buttonList = new List<GameObject>();
                for(int j = 0;j <  _stageFloorList[i].Count;j++)
                {
                    var buttonObject = Instantiate(StageButtonObject);
                    buttonObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
                    buttonList.Add(buttonObject);
                    
                    //_stageFloorList[i][j].buttonObject = buttonObject;
                    ((RectTransform)buttonObject.transform).anchoredPosition = _stageFloorList[i][j].buttonLocalPos;
                    buttonObject.transform.SetParent(floor.transform,false);
                }
                _stageButtonObjectFloorList.Add(buttonList);
            }
            //線生成
            Canvas.ForceUpdateCanvases();
            for(int i = 0;i < _stageFloorList.Count-1;i++)
            {
                for(int j = 0;j <  _stageFloorList[i].Count;j++)
                {
                    foreach(var nextNode in _stageFloorList[i][j].nextNodeList)
                    {
                        var lineObject =  Instantiate(MapLineObject);
                        lineObject.transform.SetParent(_stageButtonObjectFloorList[i][j].transform,false);
                        var SRT = (RectTransform)_stageButtonObjectFloorList[i][j].transform;
                        var ERT = (RectTransform)_stageButtonObjectFloorList[nextNode.mapIdx.Item1][nextNode.mapIdx.Item2].transform;
                        var v = ERT.position - SRT.position;
                        //回転
                        float rad = Mathf.Atan2(v.y,v.x);
                        lineObject.transform.Rotate(0f,0f,rad * Mathf.Rad2Deg);
                        float distance = Vector2.Distance(SRT.position,ERT.position)/(2f * 10f) * scrollVerticalSize;
                        ((RectTransform)lineObject.transform).sizeDelta = new Vector3(distance - Mathf.Sqrt(2f) * stageButtonSize,3f,1f);
                        ((RectTransform)lineObject.transform).anchoredPosition = distance /2f * new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
                    }
                }
            }
            //ボタンに関数を設定
            for(int i = 0;i < _stageFloorList.Count-1;i++)
            {
                for(int j = 0;j <  _stageFloorList[i].Count;j++)
                {
                    int p = i,q = j;
                    _stageButtonObjectFloorList[i][j].GetComponent<UnityEngine.UI.Button>().OnClickAsObservable()
                        .Subscribe(_ =>
                        {
                            SetStageDescription(_stageFloorList[p][q]);
                            GManager.Instance.currentStageNode = _stageFloorList[p][q];
                            _toStageButton.interactable = true;
                        })
                        .AddTo(_stageButtonObjectFloorList[p][q]);
                }
            }
        }
        public void EnableStageButton()
        {
            // int floorNum = 
            // if(floorNum == _stageFloorList.Count-1)return;
            // foreach(var stageNodeButtonObject in _stageButtonObjectFloorList[floorNum+1])
            // {
            //     stageNodeButtonObject.GetComponent<Button>().interactable = true;
            // }
            StageNode previoursStageNode = GManager.Instance.currentStageNode;
            foreach(var stageNode in previoursStageNode.nextNodeList)
            {
                var mapIdx = stageNode.mapIdx;
                _stageButtonObjectFloorList[mapIdx.Item1][mapIdx.Item2].GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
        public void SetStageDescription(StageNode stageNode)
        {
            stageNameDisplay.text = stageNode.stageName;
        }
    }
}

