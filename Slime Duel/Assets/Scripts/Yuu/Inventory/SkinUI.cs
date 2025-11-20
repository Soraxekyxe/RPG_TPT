using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yuu.Inventory
{
    public class SkinUI : MonoBehaviour
    {
        [SerializeField] 
        private Image icon;
        [SerializeField] 
        private TMP_Text rarityText;

        private InventoryUI manager;
        
        public void SetSkin(InventoryUI inventoryUI, Skin skin)
        {
            manager = inventoryUI;
            icon.sprite = skin.Icon;
            rarityText.text = skin.MySkinRarity.ToString();
        }
    }
}