using CocoDoogy.CameraSwiper;
using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.Shop.Category;
using CocoDoogy.UI.UserInfo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Shop
{
    public class ShopUI : UIPanel
    {
        [Header("Purchase Item Button")][SerializeField] private List<ShopItem> shopItems;
        [Header("Purchase Panel")][SerializeField] private PurchasePanel purchasePanel;
        [Header("Confirm Panel")][SerializeField] private ConfirmPanel confirmPanel;
        [Header("Close Button")][SerializeField] private Button closeButton;

        [Header("Categories")]
        [SerializeField] private ShopCategory itemShop;
        [SerializeField] private ShopCategory stampShop;
        [SerializeField] private ShopCategory jemShop;

        [Header("Category Select Buttons")]
        [SerializeField] private Button itemShopButton;
        [SerializeField] private Button stampShopButton;
        [SerializeField] private Button jemShopButton;

        [Header("Scroll Areas")]
        [SerializeField] private GameObject[] lists;
        
        private Transform currentActivePanel;
        
        
        //스와이프 문제 수정용
        [Header("Extra")]
        [SerializeField] private GameObject stageSelectUI;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);

            itemShopButton.onClick.AddListener(OnClickItemShopButton);
            stampShopButton.onClick.AddListener(OnClickStampShopButton);
            jemShopButton.onClick.AddListener(OnClickJemShopButton);
        }

        private void OnEnable()
        {
            InitTabs();
        }

        public override void ClosePanel()
        {
            if (!stageSelectUI.activeSelf)
            {
                PageCameraSwiper.IsSwipeable = true;
            }
            
            WindowAnimation.SwipeWindow(transform as RectTransform);
        }

        public void OpenPurchasePanel(ItemData itemData, bool isCountable) => purchasePanel.Open(itemData, isCountable, OnPurchaseRequest);
        private void OnPurchaseRequest(ItemData itemData, int quantity) => _ = ExecutePurchaseAsync(itemData, quantity);

        /// <summary>
        /// 구매를 실행하는 메서드. itemData에서 DB에 검색할 내용인 아이템ID를 가져와서 DB에 전송.<br/>
        /// PurchaseWithCashMoneyAsync를 통해서 Firebase Functions의 기능을 사용. DB 내에서 결제 처리 후 결과를 반환하여 사용함.
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="quantity"></param>
        private async Task ExecutePurchaseAsync(ItemData itemData, int quantity)
        {
            try
            {
                var result = await FirebaseManager.PurchaseWithCashMoneyAsync(itemData.itemId, quantity);

                bool success = result.ContainsKey("success") && (bool)result["success"];

                if (success)
                {
                    Debug.Log($"구매 성공: {itemData.itemName} ({quantity})");
                    confirmPanel.Open(itemData, quantity);
                }
                else
                {
                    string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                    Debug.LogWarning($"구매 실패: {reason}");
                    MessageDialog.ShowMessage("구매실패", reason, DialogMode.Confirm, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"구매 실패: {e.Message}");
            }
        }

        #region < ChangeTabs >
        private void InitTabs()
        {
            OnClickButton(itemShopButton);
            currentActivePanel = itemShop.transform;
            itemShop.gameObject.SetActive(true);
            stampShop.gameObject.SetActive(false);
            jemShop.gameObject.SetActive(false);

            for (int i = 0; i < lists.Length; ++i)
            {
                RectTransform rect = lists[i].transform as RectTransform;
                rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y);
            }
        }

        private void OnClickButton(Button clicked)
        {
            ChangeUITabs.ChangeTab(itemShopButton, false);
            ChangeUITabs.ChangeTab(stampShopButton, false);
            ChangeUITabs.ChangeTab(jemShopButton, false);

            ChangeUITabs.ChangeTab(clicked, true);
        }

        private void OnClickItemShopButton()
        {
            OnClickButton(itemShopButton);
            ToggleTab(itemShop.transform);
        }

        private void OnClickStampShopButton()
        {
            OnClickButton(stampShopButton);
            ToggleTab(stampShop.transform);
        }

        private void OnClickJemShopButton()
        {
            OnClickButton(jemShopButton);
            ToggleTab(jemShop.transform);
        }

        private void ToggleTab(Transform targetPanel)
        {
            if (currentActivePanel == targetPanel) return;

            if (currentActivePanel != null)
            {
                currentActivePanel.gameObject.SetActive(false);
            }

            targetPanel.gameObject.SetActive(true);
            currentActivePanel = targetPanel;
        }
        #endregion

        # region < InfoUI에서 버튼에 연결하기 위한 메서드 >

        public void OpenItemShopUI()
        {
            OpenPanel();
            itemShop.Change(true);
            stampShop.Change(false);
            jemShop.Change(false);
            ToggleTab(itemShop.transform);
            OnClickButton(itemShopButton);
        }
        public void OpenJemShopUI()
        {
            OpenPanel();
            itemShop.Change(false);
            stampShop.Change(false);
            jemShop.Change(true);
            ToggleTab(jemShop.transform);
            OnClickButton(jemShopButton);
        }

        public void OpenStampShopUI()
        {
            OpenPanel();
            itemShop.Change(false);
            stampShop.Change(true);
            jemShop.Change(false);
            ToggleTab(stampShop.transform);
            OnClickButton(stampShopButton);
        }
        #endregion
    }
}
