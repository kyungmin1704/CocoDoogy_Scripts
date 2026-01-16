using CocoDoogy.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Shop
{
    public class ConfirmPanel : MonoBehaviour
    {
        [SerializeField] private Image purchasedItemImage;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private CommonButton confirmButton;
        [SerializeField] private Button backGround;

        private void Awake()
        {
            confirmButton.onClick.AddListener(OnCloseWindowButtonClicked);
            backGround.onClick.AddListener(OnCloseWindowButtonClicked);
        }
        
        public void Open(ItemData itemData, int quantity)
        {
            purchasedItemImage.sprite = itemData.itemSprite;
            quantityText.text = $"x{quantity}";
            gameObject.SetActive(true);
        }

        private void OnCloseWindowButtonClicked()
        {
            WindowAnimation.CloseWindow(transform);
        }
    }
}