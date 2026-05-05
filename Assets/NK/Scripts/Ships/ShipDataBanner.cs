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
        public void SetBannerMessage(string shipName,string shipDescription)
        {
            shipNameText.text = shipName;
            shipDescriptionText.text = "・"+shipDescription;
        }
    }
}