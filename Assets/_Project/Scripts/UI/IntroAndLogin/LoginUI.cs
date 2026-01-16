using CocoDoogy.Network;
using CocoDoogy.Network.Login;
using CocoDoogy.UI.Popup;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CocoDoogy.UI.IntroAndLogin
{
    public class LoginUI : UIPanel
    {
        /// <summary>
        /// 구글로그인을 하는 버튼
        /// </summary>
        [SerializeField] private Button googleLoginButton;
        /// <summary>
        /// 익명로그인을 하는 버튼 
        /// </summary>
        [SerializeField] private Button anonymousLoginButton;

        private LoginViewModel loginVM;

        private void Awake()
        {
            Init();
        }
        private void OnEnable()
        {
            // 인터넷 연결 이벤트 구독
            NetworkMonitor.Instance.OnConnectionChanged += UpdateButtonState;
            UpdateButtonState(NetworkMonitor.Instance.IsConnected);
        }

        private void OnDisable()
        {
            // 인터넷 연결 이벤트 구독 해제
            NetworkMonitor.Instance.OnConnectionChanged -= UpdateButtonState;
        }
        /// <summary>
        /// 로그인을 위한 초기화를 하는 메서드
        /// </summary>
        private void Init()
        {
            var authProvider = new AuthProvider();
            loginVM = new LoginViewModel(authProvider);

            googleLoginButton.interactable = false;
            anonymousLoginButton.interactable = false;

            FirebaseManager.SubscribeOnFirebaseInitialized(() =>
            {
                authProvider.InitGoogleSignIn();
                googleLoginButton.interactable = true;
                anonymousLoginButton.interactable = true;
            });

            loginVM.OnUserChanged += OnUserLoggedIn;
            loginVM.OnErrorChanged += OnLoginError;

            googleLoginButton.onClick.AddListener(() =>
            {
                googleLoginButton.interactable = false;
                loginVM.SignIn();
            });
            anonymousLoginButton.onClick.AddListener(() =>
            {
                anonymousLoginButton.interactable = false;
                loginVM.SignInAnonymously();
            });
        }

        public override void ClosePanel() => gameObject.SetActive(false);

        /// <summary>
        /// 로그인에 성공 시 Lobby로 씬을 이동시키는 메서드
        /// </summary>
        /// <param name="loginUser"></param>
        private void OnUserLoggedIn(FirebaseUser loginUser)
        {
            SceneManager.LoadScene("Lobby");
        }

        private void OnLoginError(string errorMessage)
        {
            MessageDialog.ShowMessage("로그인 실패",errorMessage,DialogMode.Confirm,null);
        }
        
        /// <summary>
        /// 인터넷 연결 여부에 따라 버튼 상태 변경
        /// </summary>
        private void UpdateButtonState(bool isConnected)
        {
            googleLoginButton.interactable = isConnected;
            anonymousLoginButton.interactable = isConnected;
        }
    }
}