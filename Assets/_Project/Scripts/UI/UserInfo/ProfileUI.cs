using CocoDoogy.CameraSwiper;
using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.Network.Login;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.StageSelect;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CocoDoogy.UI.UserInfo
{
    public class ProfileUI : UIPanel
    {
        [SerializeField] private RectTransform profileWindow;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI recordText;
        [SerializeField] private Button closeButton;

        [SerializeField] private Button googleLinkButton;
        [SerializeField] private Button logOutButton;

        private LoginViewModel loginVM;
        
        private FirebaseManager Firebase => FirebaseManager.Instance;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            Init();
        }

        private void OnEnable()
        {
            _ = RefreshUIAsync();
        }
        
        private void Init()
        {
            var authProvider = new AuthProvider();
            loginVM = new LoginViewModel(authProvider);

            loginVM.OnLoggedOut += LogOut;
            loginVM.OnErrorChanged += LinkError;
            
            if (FirebaseManager.Instance.Auth.CurrentUser.IsAnonymous)
            {
                googleLinkButton.gameObject.SetActive(true);
                googleLinkButton.onClick.AddListener(CheckGoogleLinkAsync);
            }
            logOutButton.onClick.AddListener(() => loginVM.SignOut());
        }

        private void LinkError(string errorMessage)
        {
            MessageDialog.ShowMessage("연동 실패", errorMessage,DialogMode.Confirm,null);
        }

        private void LogOut()
        {
            SceneManager.LoadScene("Intro");
        }
        
        public override void ClosePanel()
        {
            WindowAnimation.CloseWindow(profileWindow);
            PageCameraSwiper.IsSwipeable = true;
        }

        private async void CheckGoogleLinkAsync()
        {
            bool success = await loginVM.LinkGoogleAccountAsync();
            
            if (!success) return;
            
            googleLinkButton.gameObject.SetActive(false);
            _ = RefreshUIAsync();
        }
        
        private async Task RefreshUIAsync()
        {
            var docRef = Firebase.Firestore
                .Collection("users").Document(Firebase.Auth.CurrentUser.UserId)
                .Collection("public").Document("profile");
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var data = snapshot.ToDictionary();
                nicknameText.text = $"닉네임 : {data["nickName"]}";
                recordText.text =
                    $"스테이지 : {StageSelectManager.LastClearedStage.theme.Hex2Int()} 테마 - {StageSelectManager.LastClearedStage.level.Hex2Int()}";
            }
            else
            {
                Debug.Log("해당 문서가 존재하지 않습니다.");
            }
        }
    }
}