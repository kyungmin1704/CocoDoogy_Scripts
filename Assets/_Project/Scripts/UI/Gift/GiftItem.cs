using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Gift
{
    public class GiftItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI giftName;
        [SerializeField] private Image giftImage;
        [SerializeField] private TextMeshProUGUI giftCount;
        [SerializeField] private Button takeGiftButton;

        private Action<string, bool> onTake;

        private string itemType;
        public void Init(string nickname, string type, string itemCount, bool allTake, Action<string, bool> takeCallback)
        {
            itemType = type;
            giftName.text = nickname;
            giftCount.text = itemCount;
            onTake = takeCallback;

            takeGiftButton.onClick.AddListener(() => onTake?.Invoke(itemType, allTake));
        }
    }
}