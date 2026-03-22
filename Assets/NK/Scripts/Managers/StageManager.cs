using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Maps;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance{get;private set;}
        public Button ToMapButton;
        //private bool isBannerPushed = false;
        private bool isBannerPushed = false;
        [Header("prefab")]
        public GameObject NoticeObject;
        [Header("UI")]
        public TextMeshProUGUI _StageNameText;
        public RectTransform _RewardContent;
        public RectTransform _ShopScrollContent;
        public GameObject _LeaveShopButton;
        [Header("Canvas")]
        public GameObject ResultCanvas;
        public GameObject _ItemBannerCanvas;
        public GameObject _ShopItemBannerCanvas;
        public GameObject _UICanvas;
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
            allItemList = Resources.LoadAll<Item>("ItemAssets").ToList();
            InstantiateStage(GManager.Instance.currentStageNode);
            gameObject.UpdateAsObservable()
                .Delay(TimeSpan.FromSeconds(3f))
                .Take(1)
                .Subscribe(_ =>
                {
                    StartCoroutine(GManager.Instance.currentStageNode.stageEncount.SetStageEncount());
                })
                .AddTo(gameObject);
            _ItemBannerCanvas.SetActive(false);
            ResultCanvas.SetActive(false);
            EventManager.OnStageClear
                .Subscribe(_ =>
                {
                    Debug.Log("Clear");
                    StageClear();
                })
                .AddTo(EventManager.Instance);
            SetNotice(GManager.Instance.currentStageNode.stageName);
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
        private void SetNotice(string message)
        {
            var notice = Instantiate(NoticeObject);
            notice.transform.SetParent(_UICanvas.transform,false);
            notice.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
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
                radius += (int)UnityEngine.Random.Range(2f,4f);
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
            _ItemBannerCanvas.SetActive(true);
            SetItemBanner(3);
            yield return new WaitUntil(()=>isBannerPushed);
            yield return new WaitForSeconds(1f);
            foreach (Transform banner in _ItemBannerCanvas.transform.GetChild(1))
            {
                //ItemBannerをすべて削除
                Destroy(banner.gameObject);
            }
            _ItemBannerCanvas.SetActive(false);

            yield break;
        }
        private void SetItemBanner(int n)
        {
            var buttonList = new List<Button>();
            var itemList = GetRandomItem(allItemList,GManager.Instance.itemList,n);
            foreach(var item in itemList)
            {
                var banner = Instantiate(_ItemBannerButton);
                var button = banner.transform.GetChild(0).GetComponent<Button>();
                buttonList.Add(button);
                banner.transform.SetParent(_RewardContent,false);
                banner.GetComponent<ItemBanner>().SetBanner(item.itemName,item.GetDescription());
                button.OnClickAsObservable()
                    .Where(_=>!isBannerPushed)
                    .Subscribe(_=>
                    {
                        var buttonObject = banner;
                        isBannerPushed = true;
                        GManager.Instance.itemList.Add(item);
                        foreach(var button in buttonList)
                        {
                            button.interactable = false;
                        }
                        buttonObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BannerUp");
                    })
                    .AddTo(banner);
            }
            // for(int i = 0;i < n;i++)
            // {
            //     var banner = Instantiate(_ItemBannerButton);
            //     var randomItem = allItemList[UnityEngine.Random.Range(0,allItemList.Count)]
            //     banner.transform.SetParent(_RewardContent,false);
            //     banner.GetComponent<ItemBanner>().SetBanner(randomItem.itemName,randomItem.GetDescription());
            //     banner.transform.GetChild(0).GetComponent<Button>().OnClickAsObservable()
            //         .Where(_=>!isBannerPushed)
            //         .Subscribe(_=>
            //         {
            //             var item = randomItem;
            //             var buttonObject = banner;
            //             isBannerPushed = true;
            //             GManager.Instance.itemList.Add(item);
            //             //ItemBannerCanvas.SetActive(false);
            //             foreach(var button in buttonList)
            //             {
            //                 button.GetComponent<Button>().interactable = false;
            //             }
            //             buttonObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BannerUp");
            //         })
            //         .AddTo(banner);
            // }   
            
        }
        private List<Item>  GetRandomItem(List<Item> allItems,List<Item> ownedItems,int count)
        {
            return allItems.Except(ownedItems).OrderBy(x => UnityEngine.Random.value).Take(count).ToList();
        }
        private void SetCredit(int value)
        {
            
        }
    }
}

