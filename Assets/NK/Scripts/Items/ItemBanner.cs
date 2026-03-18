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
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
        public void SetBanner(string itemName,string itemDescription)
        {
            itemNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
        }
    }
}

