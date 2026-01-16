using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.Gift;
using CocoDoogy.UI.Shop;
using CocoDoogy.UI.StageSelect;
using CocoDoogy.UI.UserInfo;
using System;
using System.Collections;
using UnityEngine;

namespace CocoDoogy.UI.UIManager
{
    public class LobbyUIManager : Singleton<LobbyUIManager>
    {
        [Header("MainUIs")]
        [SerializeField] private RectTransform lobbyUIPanel;
        [SerializeField] private RectTransform stageSelectUIPanel;
        
        [Header("Lobby UI Panels")]
        [SerializeField] private ProfileUI profilePanel;
        [SerializeField] private FriendUI friendPanel;
        [SerializeField] private GiftUI giftPanel;
        [SerializeField] private SettingsUI settingsPanel;
        [SerializeField] private ShopUI shopPanel; 
        [SerializeField] private InfoUI infoPanel;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton profileButton;
        [SerializeField] private CommonButton friendsButton;
        [SerializeField] private CommonButton giftsButton;
        [SerializeField] private CommonButton settingsButton;
        [SerializeField] private CommonButton shopButton;
        [SerializeField] private CommonButton startButton;
        
        public ShopUI ShopUI => shopPanel;
        
        protected override void Awake()
        {
            base.Awake();
            stageSelectUIPanel.gameObject.SetActive(false);
            
            profileButton.onClick.AddListener(OnClickProfileButton);
            friendsButton.onClick.AddListener(OnClickFriendButton);
            giftsButton.onClick.AddListener(OnClickGiftButton);
            settingsButton.onClick.AddListener(OnClickSettingButton);
            shopButton.onClick.AddListener(OnClickShopButton);
            startButton.onClick.AddListener(OnStartButtonClicked);
            
            // StartCoroutine(FirebaseManager.Instance.UpdateTicketCoroutine());
            _ = FirebaseManager.Instance.RechargeTicketAsync();
        }

        private async void OnEnable()
        {
            StageSelectManager.LastClearedStage = await FirebaseManager.GetLastClearStage(FirebaseManager.Instance.Auth.CurrentUser.UserId);
        }
        
        private IEnumerator Start()
        {
            // 해당 이벤트 추가는 로그인 후 되어야 되므로 UIManager에서 구독, 나중에 문제되면 DataManager.Instance가 null 아닐때 async로 변경해서 사용
            yield return new WaitUntil(() => DataManager.Instance != null);
            DataManager.Instance.OnPrivateUserDataLoaded += friendPanel.FriendsInfoPanel.Refresh;
            DataManager.Instance.OnPrivateUserDataLoaded += friendPanel.ReceivedRequestPanel.Refresh;
            DataManager.Instance.OnPrivateUserDataLoaded += friendPanel.SentRequestPanel.Refresh;
            DataManager.Instance.OnPrivateUserDataLoaded += giftPanel.SubscriptionEvent;
            DataManager.Instance.OnPrivateUserDataLoaded += infoPanel.SubscriptionEvent;
            
            // 씬 이동 후 이벤트가 구독되기 전에 실행되서 UI Refresh가 되지 않아 따로 이벤트 실행
            DataManager.Instance.InvokePrivateUserData();
        }

        private void OnDisable()
        {
            DataManager.Instance.OnPrivateUserDataLoaded -= friendPanel.FriendsInfoPanel.Refresh;
            DataManager.Instance.OnPrivateUserDataLoaded -= friendPanel.ReceivedRequestPanel.Refresh;
            DataManager.Instance.OnPrivateUserDataLoaded -= friendPanel.SentRequestPanel.Refresh;
            DataManager.Instance.OnPrivateUserDataLoaded -= giftPanel.SubscriptionEvent;
            DataManager.Instance.OnPrivateUserDataLoaded -= infoPanel.SubscriptionEvent;
        }
        
        private void OnClickProfileButton() => profilePanel.OpenPanel();
        private void OnClickFriendButton() => friendPanel.OpenPanel();
        private void OnClickGiftButton() =>  giftPanel.OpenPanel();
        private void OnClickSettingButton() => settingsPanel.OpenPanel();
        private void OnClickShopButton() => shopPanel.OpenPanel();

        private void OnStartButtonClicked()
        {
            if (stageSelectUIPanel.gameObject.activeSelf) return;
            WindowAnimation.SwipeWindow(lobbyUIPanel);
            stageSelectUIPanel.gameObject.SetActive(true);
        }

    }
}
