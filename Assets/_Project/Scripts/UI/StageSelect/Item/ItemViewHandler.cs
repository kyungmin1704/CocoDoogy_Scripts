using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.UIManager;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Item
{
    public class ItemViewHandler : MonoBehaviour
    {
        [Header("UI Elements")] [SerializeField]
        private ItemButton[] itemInfoButtons;

        /// <summary>
        /// 스테이지를 선택하면 초기화를 하도록 OnEnable에서 초기화를 담당<br/>
        /// </summary>

        private void Awake()
        {
            DataManager.Instance.OnPrivateUserDataLoaded += Refresh;
        }

        private void OnEnable()
        {
            InitAsync();
        }

        private void OnDisable()
        {
            ClearToggleEvent();
            DataManager.Instance.OnPrivateUserDataLoaded -= Refresh;
        }

        /// <summary>
        /// 스테이지 버튼을 클릭해서 StageUI가 나오게 되면 UI를 초기화 하는 메서드
        /// </summary>
        private async void InitAsync()
        {
            IDictionary<string, object> itemDictionary = await FirebaseManager.GetItemListAsync();
            IReadOnlyList<ItemData> itemData = DataManager.Instance.ItemData;
            for (int i = 0; i < itemDictionary.Count; i++)
            {
                ItemData initItem = itemData[i];
                itemInfoButtons[i].Init(itemDictionary, initItem);
                itemInfoButtons[i].OnClickEvent += OnClickButton;
            }
        }
        private async void Refresh()
        {
            IDictionary<string, object> itemDictionary = await FirebaseManager.GetItemListAsync();
            IReadOnlyList<ItemData> itemData = DataManager.Instance.ItemData;
            for (int i = 0; i < itemDictionary.Count; i++)
            {
                ItemData initItem = itemData[i];
                itemInfoButtons[i].Init(itemDictionary, initItem);
            }
        }

        private void ClearToggleEvent()
        {
            foreach (var button in itemInfoButtons)
            {
                button.OnClickEvent -= OnClickButton;
            }
        }

        /// <summary>
        /// 토글 클릭 시 발생하는 이벤트 메서드
        /// </summary>
        /// <param name="itemButton"></param>
        private void OnClickButton(ItemButton itemButton)
        {
            InfoDialog.ShowInfo("아이템 정보", itemButton.ItemData.itemName,itemButton.ItemData.itemDescription, itemButton.ItemData.itemSprite, DialogMode.Confirm, OpenShopUI, "상점");
        }

        private void OpenShopUI(CallbackType callbackType)
        {
            if (callbackType == CallbackType.Yes)
                LobbyUIManager.Instance.ShopUI.OpenItemShopUI();
        }
    }
}
