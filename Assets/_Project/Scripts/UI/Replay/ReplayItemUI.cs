using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.UI.Replay
{
    public class ReplayItemUI : MonoBehaviour
    {
        [SerializeField] private ReplayItemButton[] itemButtons;
        
        private void OnEnable()
        {
            ItemHandler.OnValueChanged += OnItemValueChanged;
        }

        private void OnDisable()
        {
            ItemHandler.OnValueChanged -= OnItemValueChanged;
        }
        
        private void Start()
        {
            for (int i = 0; i < itemButtons.Length; i++)
            {
                ItemData itemData = DataManager.Instance.ItemData[i];
                itemButtons[i].ItemData = itemData;
                
                ItemHandler.SetValue(itemData, true);
            }
        }
        
        /// <summary>
        /// 아이템 사용여부에 따라서 버튼의 상태를 변화시키는 메서드
        /// </summary>
        private void OnItemValueChanged(ItemData item, bool value)
        {
            float rgb = value ? 1f : 0.2f;
            
            foreach (var button in itemButtons)
            {
                if (button.ItemData == item)
                {
                    if (button.Button)
                    {
                        button.ButtonColor.DOColor(new Color(rgb,rgb,rgb), 0.2f);
                    }
                }
            }
        }
    }
}