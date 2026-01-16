using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect.Item
{
    public class ItemButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button itemButton;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        /// <summary>
        /// 현재 해당 토글이 가리키는 아이템 데이터, 사용 안하면 삭제.
        /// </summary>
        public IDictionary<string, object> Data { get; private set; }

        public Action<ItemButton> OnClickEvent {get; set;}
        
        /// <summary>
        /// 현재 해당 토글이 가리키는 아이템의 수량
        /// </summary>
        private int CurrentAmount {get; set;}
        
        public ItemData ItemData {get; set;}
        
        
        
        private void Awake()
        {
            // 실수로 Inspector에서 연결을 안했을 때를 대비한 코드 
            if (!itemButton) itemButton = GetComponent<Button>();
            if (!itemAmountText) itemAmountText = GetComponentInChildren<TextMeshProUGUI>();
            
            itemButton.onClick.AddListener(() =>
            {
                OnClickEvent?.Invoke(this);
            });

            DataManager.Instance.OnPrivateUserDataLoaded += Refresh;
        }

        public void Init(IDictionary<string, object> data, ItemData item)
        {
            Data = data;
            ItemData = item;
            
            if (data.TryGetValue(item.itemId, out object value))
            {
                CurrentAmount = Convert.ToInt32(value);   
            }
            DataManager.Instance.CurrentItem[item] = CurrentAmount;
            itemAmountText.text = $"{CurrentAmount}";

            
            var buttonColor = itemButton.GetComponentsInChildren<Graphic>();
            if (CurrentAmount <= 0)
            {
                foreach (var colors in buttonColor)
                {
                    colors.DOColor(new Color(0.75f, 0.75f, 0.75f), 0.2f);
                }
            }
            else
            {
                foreach (var colors in buttonColor)
                {
                    colors.DOColor(Color.white, 0.2f);
                }
            }
        }

        public void Refresh()
        {
            if (Data.TryGetValue(ItemData.itemId, out object value))
            {
                CurrentAmount = Convert.ToInt32(value);   
            }
            itemAmountText.text = $"{CurrentAmount}";
        }
    }
}
