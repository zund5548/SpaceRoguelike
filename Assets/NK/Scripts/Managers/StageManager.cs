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
using Weapons;

namespace Managers
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance{get;private set;}
        public Button ToMapButton;
        public Button GameClearButton;
        //private bool isBannerPushed = false;
        private bool isBannerPushed = false;
        [Header("prefab")]
        public GameObject NoticeObject;
        [Header("UI")]
        public TextMeshProUGUI _CurrentFloorText;
        public TextMeshProUGUI _StageNameText;
        public RectTransform _RewardContent;
        public RectTransform _ShopScrollContent;
        public GameObject _LeaveShopButton;
        [Header("Canvas")]
        public GameObject _ResultCanvas;
        public GameObject _GameClearCanvas;
        public GameObject _GameFailCanvas;
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
            if(GManager.Instance.currentStageNode.stageType == StageNode.StageType.Boss)SetGameClear();
            else SetToMapButton();
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
            _ResultCanvas.SetActive(false);

            SetNotice(GManager.Instance.currentStageNode.stageName);
            SetFloorDisplay();
            GManager.Instance.SetCreditDisplay();
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
        private void SetGameClear()
        {
            if(GameClearButton == null)return;
            GameClearButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GameClearButton.interactable = false;
                    SceneLoader.Instance.ToLobby();
                })
                .AddTo(GameClearButton.gameObject);
        }
        private void SetNotice(string message)
        {
            var notice = Instantiate(NoticeObject);
            notice.transform.SetParent(_UICanvas.transform,false);
            notice.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        }
        private void SetFloorDisplay()
        {
            string floorNum = GManager.Instance.currentStageNode.floorStageNum.ToString();
            _CurrentFloorText.text = "セクター"+floorNum;
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
        public void StageClear()
        {
            if(EventManager.Instance.FailDisposable != null)EventManager.Instance.FailDisposable.Dispose();
            StartCoroutine(ClearCoroutine());
            ShipManager.Instance.DeleteAllPlayer();
        }
        public void GameClear()
        {
            if(EventManager.Instance.FailDisposable != null)EventManager.Instance.FailDisposable.Dispose();
            ShipManager.Instance.DeleteAllPlayer();
            _GameClearCanvas.SetActive(true);
            GManager.Instance.ResetManager();
        }
        public void GameFail()
        {
            if(EventManager.Instance.ClearDisposable != null)EventManager.Instance.ClearDisposable.Dispose();
            _GameFailCanvas.SetActive(true);
            GManager.Instance.ResetManager();
        }
        private IEnumerator ClearCoroutine()
        {
            //アイテムの詳細をItemBannerButtonにセットする
            yield return SetItemCoroutine();
            //ボタンを押したら続行
            DeleteAllPlanet();
            _ResultCanvas.SetActive(true);
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
            var itemList = GetRandomItem(GManager.Instance.itemList,n);
            if(itemList == null || itemList.Count == 0)isBannerPushed = true;
            foreach(var item in itemList)
            {
                var banner = Instantiate(_ItemBannerButton);
                var button = banner.transform.GetChild(0).GetComponent<Button>();
                buttonList.Add(button);
                banner.transform.SetParent(_RewardContent,false);
                banner.GetComponent<ItemBanner>().SetBanner(item.itemName,item.GetItemDescription());
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
                            //buttonObject.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        buttonObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BannerUp");
                    })
                    .AddTo(banner);
            }
        }
        public List<Item>  GetRandomItem(List<Item> ownedItems,int count)
        {
            List<Item> itemList = new();
            List<Item> tmpItemList = new();
            List<Type> uniqueStatModifyList = new List<Type>
            {
                typeof(BulletStatModify),
                typeof(MissileStatModify),
                typeof(DroneStatModify),
            };
            itemList = allItemList
                .Where(item =>
                    !item.itemEffectList.Any(effect =>
                        uniqueStatModifyList.Any(t => t.IsAssignableFrom(effect.GetType()))
                    )
                )
                .ToList();
            foreach(var shipData in GManager.Instance.playerShipDataList)
            {
                switch(shipData.weaponData)
                {
                    case BulletShot:
                        tmpItemList = allItemList.Where(item => item.itemEffectList.Any(itemEffect => itemEffect is BulletStatModify)).ToList();
                        break;
                    case MissileShot:
                        tmpItemList = allItemList.Where(item => item.itemEffectList.Any(itemEffect => itemEffect is MissileStatModify)).ToList();
                        break;
                    case DroneLaunch:
                        tmpItemList = allItemList.Where(item => item.itemEffectList.Any(itemEffect => itemEffect is DroneStatModify)).ToList();
                        break;
                }
                itemList = itemList.Union(tmpItemList).ToList();
                tmpItemList.Clear();
            }
            return itemList.Except(ownedItems).OrderBy(x => UnityEngine.Random.value).Take(count).ToList();
            //return allItemList.OrderBy(x => .Random.value).Take(count).ToList();
        }
        
    }
}

