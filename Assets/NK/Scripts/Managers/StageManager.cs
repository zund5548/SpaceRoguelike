using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Maps;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Managers
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance{get;private set;}
        public Button ToMapButton;
        private bool isBannerPushed = false;
        private bool isAnimEnd = false;
        [Header("Canvas")]
        public GameObject ResultCanvas;
        public GameObject ItemBannerCanvas;

        public GameObject _planetObject;
        public GameObject _ItemBannerButton;
        public List<Item> allItemList;
        List<GameObject> planetObjectList = new();
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
            // _planetObject = (GameObject)Resources.Load("PlanetObject");
            // _ItemBannerButton = (GameObject)Resources.Load("ItemBannerButton");
            SetToMapButton();
            InstantiateStage(GManager.Instance.currentStageNode);
            gameObject.UpdateAsObservable()
                .Delay(TimeSpan.FromSeconds(3f))
                .Take(1)
                .Subscribe(_ =>
                {
                    GManager.Instance.currentStageNode.stageEncount.SetStageEncount();
                })
                .AddTo(gameObject);
            ItemBannerCanvas.SetActive(false);
            ResultCanvas.SetActive(false);
            EventManager.OnStageClear
                .Subscribe(_ =>
                {
                    Debug.Log("Clear");
                    StageClear();
                })
                .AddTo(EventManager.Instance);
        }
        private void SetToMapButton()
        {
            if(ToMapButton == null)return;
            ToMapButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ToMapButton.interactable = false;
                    GManager.Instance.passsedStageNodes.Add(GManager.Instance.currentStageNode);
                    SceneLoader.Instance.ToMap();
                })
                .AddTo(ToMapButton.gameObject);
        }

        private void InstantiateStage(StageNode stageNode)
        {
            if(stageNode.planetList.Count == 0)return;
            //恒星
            // var starObject = Instantiate(_planetObject);
            // Star star = (Star)stageNode.planetList[0];    
            // starObject.name = "StarObject";
            // starObject.transform.position = Vector2.zero;
            // starObject.transform.localScale = star.radius * Vector2.one;
             
            // if(star.sprite != null)star.GetComponent<SpriteRenderer>().sprite = star.sprite;
            //惑星
            
            foreach(var star in stageNode.starList)
            {
                var planetObject = Instantiate(_planetObject);
                planetObjectList.Add(planetObject);
                planetObject.transform.position = Vector2.zero;
                planetObject.transform.localScale = star.radius * Vector2.one;
            }   
            float radius = 3f;
            int pointNum = 100;
            foreach(var planet in stageNode.planetList)
            {
                var planetObject = Instantiate(_planetObject);
                planetObjectList.Add(planetObject);
                float rad = UnityEngine.Random.Range(-Mathf.PI,Mathf.PI);
                planetObject.transform.position = radius * new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
                planetObject.transform.localScale = planet.radius * Vector2.one;
                var LR = planetObject.GetComponent<LineRenderer>();
                LR.positionCount = pointNum;
                float orbitRad = 0f;
                for(int i = 0;i < pointNum;i++)
                {
                    LR.SetPosition(i,radius * new Vector3(Mathf.Cos(orbitRad),Mathf.Sin(orbitRad),0f));
                    orbitRad += 2f * Mathf.PI/pointNum;
                }
                radius += UnityEngine.Random.Range(2f,4f);
            }   
        }
        private void DeleteAllPlanet()
        {
            int n = planetObjectList.Count;
            for(int i = 0;i < n;i++)
            {
                Destroy(planetObjectList[i]);
            }
            planetObjectList.Clear();
        }
        private void SetItemBanner(int n)
        {
            allItemList = Resources.LoadAll<Item>("ItemAssets").ToList();
            var buttonList = new List<Button>();
            for(int i = 0;i < n;i++)
            {
                var banner = Instantiate(_ItemBannerButton);
                var randomItem = allItemList[UnityEngine.Random.Range(0,allItemList.Count)];
                banner.transform.SetParent(ItemBannerCanvas.transform.GetChild(0).GetChild(1),false);
                banner.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = randomItem.itemName;
                banner.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = randomItem.GetDescription();
                banner.GetComponent<Button>().OnClickAsObservable()
                    .Where(_=>!isBannerPushed)
                    .Subscribe(_=>
                    {
                        var item = randomItem;
                        var buttonObject = banner;
                        isBannerPushed = true;
                        GManager.Instance.itemList.Add(item);
                        StartCoroutine(BannerAnimation(buttonObject));
                        ItemBannerCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        foreach(var button in buttonList)
                        {
                            button.GetComponent<Button>().interactable = false;
                        }
                    })
                    .AddTo(banner);
            }
            
        }

        private void StageClear()
        {
            StartCoroutine(ClearCoroutine());
            ShipManager.Instance.DeleteAllPlayer();
        }

        private IEnumerator ClearCoroutine()
        {
            //アイテムの詳細をItemBannerButtonにセットする
            yield return SetItemCoroutine();
            //ボタンを押したら続行
            DeleteAllPlanet();
            ResultCanvas.SetActive(true);
            yield break;
        }

        private IEnumerator SetItemCoroutine()
        { 
            ItemBannerCanvas.SetActive(true);
            SetItemBanner(3);
            yield return new WaitUntil(()=>isAnimEnd);
            ItemBannerCanvas.SetActive(false);
            yield break;
        }

        private IEnumerator BannerAnimation(GameObject banner)
        {
            float deg = 0f;
            while(true)
            {
                deg += 180f * Time.deltaTime;
                ((RectTransform)banner.transform).anchoredPosition += new Vector2(0f,5f * Mathf.Cos(deg * Mathf.Deg2Rad));
                if(deg > 90f)break;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            isAnimEnd = true;
            yield break;
        }
    }
}

