using UnityEngine;

namespace CocoDoogy.Data
{
    [CreateAssetMenu(fileName ="New Item", menuName = "InGameData/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Default Item Data")]
        [Tooltip("아이템 이름")] 
        public string itemName;
        [Tooltip("아이템 이미지")]
        public Sprite itemSprite;
        [Tooltip("아이템 구매 가격")]
        public int purchasePrice;
        [Tooltip("아이템 설명")][TextArea(3,10)]
        public string itemDescription;
        [Tooltip("DB 아이템 코드")]
        public string itemId;
        [Tooltip("아이템 효과")]
        public ItemEffect effect;
        private void Reset()
        {
            itemName = string.Empty;
            purchasePrice = 0;
            itemDescription = string.Empty;
        }
    }
}
