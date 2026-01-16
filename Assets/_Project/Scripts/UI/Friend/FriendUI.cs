using CocoDoogy.CameraSwiper;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.UserInfo;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Friend
{
    public class FriendUI : UIPanel
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform friendsWindow;
        [SerializeField] private GameObject searchWindowBg;

        [Header("Close Button")]
        [SerializeField] private Button closeButton;

        [Header("Tab Buttons")]
        [SerializeField] private Button friendsInfoButton;
        [SerializeField] private Button friendsRequestButton;
        [SerializeField] private Button friendsSentButton;

        [Header("Popup Buttons")]
        [SerializeField] private CommonButton searchFriendButton;
        [SerializeField] private CommonButton sendAllButton;

        [Header("Tabs")]
        [SerializeField] private FriendsInfoPanel friendsInfoPanel;
        [SerializeField] private ReceivedRequestPanel receivedRequestPanel;
        [SerializeField] private SentRequestPanel sentRequestPanel;

        [Header("Take All Gift Button")]
        [SerializeField] private CommonButton takeAllGiftButton;
        
        public FriendsInfoPanel FriendsInfoPanel => friendsInfoPanel;
        public ReceivedRequestPanel ReceivedRequestPanel => receivedRequestPanel;
        public SentRequestPanel SentRequestPanel => sentRequestPanel;

        /// <summary>
        /// 현재 열려있는 친구 탭 패널
        /// </summary>
        private RequestPanel currentActivePanel;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            friendsInfoButton.onClick.AddListener(OnClickFriendInfoButton);
            friendsRequestButton.onClick.AddListener(OnClickFriendRequestButton);
            friendsSentButton.onClick.AddListener(OnClickFriendSentButton);
            searchFriendButton.onClick.AddListener(OnClickFriendSearch);
            takeAllGiftButton.onClick.AddListener(OnClickSendGiftAllFriendAsync);
        }
        private void OnEnable()
        {
            InitTabs();
            WindowAnimation.CloseWindow(searchWindowBg.transform);
        }

        public override void ClosePanel()
        {
            WindowAnimation.SwipeWindow(friendsWindow);
            PageCameraSwiper.IsSwipeable = true;
        }

        private void OnClickFriendSearch() => searchWindowBg.SetActive(true);

        #region ChangeTabs
        private void InitTabs()
        {
            currentActivePanel = friendsInfoPanel;
            OnClickButton(friendsInfoButton);
            friendsInfoPanel.gameObject.SetActive(true);
            receivedRequestPanel.gameObject.SetActive(false);
            sentRequestPanel.gameObject.SetActive(false);
        }
        private void OnClickFriendInfoButton()
        {
            OnClickButton(friendsInfoButton);
            ToggleTab(friendsInfoPanel);
        }

        private void OnClickFriendRequestButton()
        {
            OnClickButton(friendsRequestButton);
            ToggleTab(receivedRequestPanel);
        }

        private void OnClickFriendSentButton()
        {
            OnClickButton(friendsSentButton);
            ToggleTab(sentRequestPanel);
        }

        private void ToggleTab(RequestPanel targetPanel)
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
        private void OnClickButton(Button clicked)
        {
            ChangeUITabs.ChangeTab(friendsInfoButton, false);
            ChangeUITabs.ChangeTab(friendsRequestButton, false);
            ChangeUITabs.ChangeTab(friendsSentButton, false);

            ChangeUITabs.ChangeTab(clicked, true);
        }
        
        private async void OnClickSendGiftAllFriendAsync()
        {
            try
            {
                int sentCount = await FirebaseManager.SendGiftToAllFriendsAsync();
                MessageDialog.ShowMessage(
                    sentCount != 0 ? "선물 보내기 성공" : "선물 보내기 실패",
                    sentCount != 0 ? $"{sentCount}명의 친구에게 선물을 보냈습니다." : "이미 모든 친구에게 선물을 보냈습니다.",
                    DialogMode.Confirm,
                    null);
            }
            catch (Exception ex)
            {
                MessageDialog.ShowMessage("선물 보내기 실패", ex.Message,DialogMode.Confirm, null);
            }
        }
    }
}