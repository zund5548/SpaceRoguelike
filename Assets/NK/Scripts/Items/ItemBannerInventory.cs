using TMPro;
using UnityEngine;

namespace Items
{
    public class ItemBannerInventory : MonoBehaviour
    {
        public TextMeshProUGUI _ItemNameTMP;
        public TextMeshProUGUI _ItemDescriptionTMP;
        public void SetItemBanner(Item item)
        {
            _ItemNameTMP.text = item.itemName;
            foreach(var itemEffect in item.itemEffectList)
            {
                _ItemDescriptionTMP.text += "・" + itemEffect.description + "\n";
            }
        }
    }
}

