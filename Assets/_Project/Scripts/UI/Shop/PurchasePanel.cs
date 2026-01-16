using CocoDoogy.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Shop
{
    public class PurchasePanel : MonoBehaviour
    {
        [Header("Selected Item Info")]
        [SerializeField] private Image selectedItemImage;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        [Header("Counter")]
        [SerializeField] private GameObject counter;
        
        [Header("Decrease Purchase Item Quantity (-1, -10)")]
        [SerializeField] private CommonButton decrease1Button;
        [SerializeField] private CommonButton decrease10Button;
        
        [Header("Increase Purchase Item Quantity (+1, +10)")]
        [SerializeField] private CommonButton increase1Button;
        [SerializeField] private CommonButton increase10Button;
        
        [Header("Current Purchase Item Quantity")]
        [SerializeField] private TextMeshProUGUI currentQuantityText;
        
        [Header("Purchase & Cancel Buttons")]
        [SerializeField] private CommonButton purchaseButton;
        [SerializeField] private CommonButton cancelButton;
        [SerializeField] private Button backGround;

        private ItemData currentItem;
        private int quantity = 1;
        private Action<ItemData, int> onPurchaseRequest;
        
        private void Awake()
        {
            decrease1Button.onClick.AddListener(() => ChangeQuantity(-1));
            decrease10Button.onClick.AddListener(() => ChangeQuantity(-10));
            increase1Button.onClick.AddListener(() => ChangeQuantity(+1));
            increase10Button.onClick.AddListener(() => ChangeQuantity(+10));

            purchaseButton.onClick.AddListener(OnClickPurchase);
            cancelButton.onClick.AddListener(OnClickCancel);
            backGround.onClick.AddListener(OnClickCancel);
        }

        public void Open(ItemData itemData, bool isCountable, Action<ItemData, int> purchaseCallback)
        {
            currentItem = itemData;
            onPurchaseRequest = purchaseCallback;
            quantity = 1;

            selectedItemImage.sprite = itemData.itemSprite;
            descriptionText.text = itemData.itemDescription;
            currentQuantityText.text = quantity.ToString();

            if (!isCountable) counter.SetActive(false);
            else counter.SetActive(true);
            gameObject.SetActive(true);
        }

        private void OnClickPurchase()
        {
            onPurchaseRequest?.Invoke(currentItem, quantity);
            gameObject.SetActive(false);
        }

        private void OnClickCancel()
        {
            WindowAnimation.CloseWindow(transform);
        }

        private void ChangeQuantity(int delta)
        {
            quantity = Mathf.Clamp(quantity + delta, 1, 100);
            currentQuantityText.text = quantity.ToString();
        }
    }
}