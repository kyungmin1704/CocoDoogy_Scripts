using CocoDoogy.Network;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.UserInfo
{
    public class SearchUI : UIPanel
    {
        [Header("UI Elements")]
        [SerializeField] private SearchWindowPopup searchWindow;

        [Header("Search Window Popup Buttons")] 
        [SerializeField] private Button closeThisButton;
        [SerializeField] private CommonButton searchButton;

        private void Awake()
        {
            closeThisButton.onClick.AddListener(ClosePanel);
            searchButton.onClick.AddListener(OnSearchButtonClicked);
        }

        private void OnEnable()
        {
            searchWindow.gameObject.SetActive(true);
        }
        
        
        public override void ClosePanel() => WindowAnimation.CloseWindow(searchWindow.transform);
        

        private void OnSearchButtonClicked()
        {
            searchWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
            MessageDialog.ShowMessage("친구 추가",
                $"{searchWindow.InputNickname}님에게 친구 추가 요청을 보내시겠습니까?",
                DialogMode.YesNo,
                OnSendButtonClickedAsync);
        }

        /// <summary>
        /// 입력된 닉네임을 받아와 UID로 변경하여 DB에서 해당 유저에게 친구 요청을 하는 메서드
        /// </summary>
        private async void OnSendButtonClickedAsync(CallbackType callbackType)
        {
            if (callbackType == CallbackType.No)
            {
                gameObject.SetActive(false);
                return;
            }

            var uid = await FirebaseManager.FindUserByNicknameAsync(searchWindow.InputNickname);
            var result = await FirebaseManager.CallFriendFunctionAsync("sendFriendsRequest", uid, "친구 요청 보내기 실패");

            bool success = (bool)result["success"];
            if (success)
            {
                MessageDialog.ShowMessage("친구 추가 요청 성공", "친구 추가 요청을 보내는데 성공했습니다.", DialogMode.Confirm, null); 
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 추가 요청 실패", reason, DialogMode.Confirm, null);
            }

            gameObject.SetActive(false);
        }
    }
}