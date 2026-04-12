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
        private List<TextMeshProUGUI> _PriceDisplayTexts = new();
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
        private Dictionary<int,int> itemPriceDic = new Dictionary<int, int>
        {
            {0,600},
            {1,1200},
            {2,1800}
        };
        private void SetShopBanner(int n)
        {
            var itemList = StageManager.Instance.GetRandomItem(GManager.Instance.itemList,n);
            foreach(var item in itemList)
            {
                var banner = Instantiate(_ItemBannerButton);
                var button = banner.transform.GetChild(0).GetComponent<Button>();
                //buttonList.Add(button);
                banner.transform.SetParent(StageManager.Instance._ShopScrollContent,false);
                banner.GetComponent<ItemBanner>().SetBanner(item.itemName,item.GetItemDescription());
                //金額表示
                var priceDisplay = UnityEngine.Object.Instantiate(_PriceDisplay);
                priceDisplay.transform.SetParent(banner.transform,false);
                ((RectTransform)priceDisplay.transform).anchoredPosition = new Vector2(0,300f);
                var priceDisplayMesh = priceDisplay.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                _PriceDisplayTexts.Add(priceDisplayMesh);
                priceDisplayMesh.text = itemPriceDic[item.itemTier].ToString();
                if(GManager.Instance.credit < itemPriceDic[item.itemTier])priceDisplayMesh.color = Color.red;
                else priceDisplayMesh.color = Color.white;
                //
                button.OnClickAsObservable()
                    .Subscribe(_=>
                    {
                        int itemPrice = itemPriceDic[item.itemTier];
                        if(GManager.Instance.credit < itemPrice)return;
                        GManager.Instance.UseCredit(itemPrice);
                        var buttonObject = banner;
                        GManager.Instance.itemList.Add(item);
                        buttonObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
                        for(int i = 0;i < itemList.Count;i++)
                        {
                            if(GManager.Instance.credit < itemPriceDic[itemList[i].itemTier])_PriceDisplayTexts[i].color = Color.red;
                            else _PriceDisplayTexts[i].color = Color.white;
                        }
                    })
                    .AddTo(banner);
            }
            //ショップを退出するボタンの設定
            var leaveShopButton =  StageManager.Instance._LeaveShopButton.GetComponent<Button>();
            if(itemList == null || itemList.Count == 0)
            {
                StageManager.Instance._ShopItemBannerCanvas.SetActive(false);
                leaveShopButton.interactable = false;
                EventManager.Instance.PublishClear();
            }
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

