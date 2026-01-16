using CocoDoogy.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CocoDoogy.UI.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [Header("Purchase Item Info")]
        [SerializeField] private ItemData itemData;
        
        [Header("Shop Item")] 
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemPrice;
        [SerializeField] private CommonButton purchaseButton;

        [SerializeField] private bool isCountable = false;
        [SerializeField] private bool isSaleForCash = false;
        public ItemData ItemData => itemData;

        
        private void OnEnable()
        {
            itemImage.sprite = itemData.itemSprite;
            itemName.text = itemData.itemName;
            if (!isSaleForCash)
            {
                itemPrice.alignment = TextAlignmentOptions.Midline;
                itemPrice.text = $"<size=200%><voffset=10><sprite name=Jem></voffset></size><space=-0.8em> {itemData.purchasePrice:N0}";
            }
            else
            {
                itemPrice.text = $"₩ {itemData.purchasePrice:N0}";
            }
        }
        
        /// <summary>
        /// 버튼 이벤트 구독하는 메서드
        /// </summary>
        /// <param name="action"></param>
        public void OnClickSubscriptionEvent(UnityAction<ItemData, bool> action) => purchaseButton.onClick.AddListener(() => action(itemData, isCountable));
    }
}