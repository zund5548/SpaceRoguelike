using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class ItemBanner : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;
        public TextMeshProUGUI itemTierDisplay;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
        public void SetBannerMessage(Item item)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.GetItemDescription();
            // for(int i = 0;i < item.itemTier + 1;i++)tierText += "★";
            itemTierDisplay.text = "tier : " + item.itemTier.ToString();
        }
    }
}

