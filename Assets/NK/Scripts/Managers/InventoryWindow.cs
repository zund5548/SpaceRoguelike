using Items;
using TMPro;
using UnityEngine;
namespace Managers
{
    public class InventoryWindow:MonoBehaviour
    {
        public TextMeshProUGUI _CreditDisplayTMP;
        public Transform _ItemDisplayContent;
        [Header("prefab")]
        public GameObject _ItemBanner;
        public void SetCreditDisplay()
        {
            _CreditDisplayTMP.SetText(GManager.Instance.credit.ToString() + "C");
        }
        public void GenerateItemBanner(Item item)
        {
            var banner = Instantiate(_ItemBanner);
            banner.transform.SetParent(_ItemDisplayContent,false);
            banner.GetComponent<ItemBannerInventory>().SetItemBanner(item);
        }
    }
}

