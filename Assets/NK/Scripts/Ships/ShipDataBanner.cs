using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Weapons;


namespace Ships
{
    public class ShipDataBanner : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI shipNameText;
        public TextMeshProUGUI shipDescriptionText;
        public void SetBannerMessage(string itemName,string itemDescription)
        {
            shipNameText.text = itemName;
            shipDescriptionText.text = "・"+itemDescription;
        }
    }
}