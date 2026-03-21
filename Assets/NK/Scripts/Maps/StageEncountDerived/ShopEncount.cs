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
namespace Maps
{
    [CreateAssetMenu(menuName = "StageEncount/ShopEncount")]
    [Serializable]
    public class ShopEncount : StageEncount
    {
        public GameObject _ItemBannerButton;
        public GameObject _PriceDisplay;
        public override IEnumerator SetStageEncount()
        {
            yield return ShopCoroutine();
        }
        public IEnumerator ShopCoroutine()
        {
            ShipManager.Instance.DeleteAllPlayer();
            SetShopBanner(6);
            StageManager.Instance._ShopItemBannerCanvas.SetActive(true);
            yield return null;
        }
        private void SetShopBanner(int n)
        {
            var buttonList = new List<Button>();
            List<Item> allItemList = Resources.LoadAll<Item>("ItemAssets").ToList();
            //アイテムボタンの設定
            for(int i = 0;i < n;i++)
            {
                var banner = UnityEngine.Object.Instantiate(_ItemBannerButton);
                var randomItem = allItemList[UnityEngine.Random.Range(0,allItemList.Count)];
                banner.transform.SetParent(StageManager.Instance._ShopScrollContent,false);
                banner.GetComponent<ItemBanner>().SetBanner(randomItem.itemName,randomItem.GetDescription());
                var priceDisplay = UnityEngine.Object.Instantiate(_PriceDisplay);
                priceDisplay.transform.SetParent(banner.transform,false);
                ((RectTransform)priceDisplay.transform).anchoredPosition = new Vector2(0,300f);
                banner.transform.GetChild(0).GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_=>
                    {
                        var item = randomItem;
                        var buttonObject = banner;
                        GManager.Instance.itemList.Add(item);
                        buttonObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
                    })
                    .AddTo(banner);
            }   
            //ショップを退出するボタンの設定
            var leaveShopButton =  StageManager.Instance._LeaveShopButton.GetComponent<Button>();
            leaveShopButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    StageManager.Instance._ShopItemBannerCanvas.SetActive(false);
                    leaveShopButton.interactable = false;
                    EventManager.Instance.PublishClear();
                })
                .AddTo(leaveShopButton.gameObject);
        }
    }   
}

