using Firebase;
using Firebase.Auth;
using Google;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network.Login
{
    public class AuthProvider
    {
        private FirebaseManager Firebase => FirebaseManager.Instance;

        private readonly string webClientID =
            "285118742247-qfbhhk07114e287uu9b7pcj8kk7brotf.apps.googleusercontent.com";

        public event Action<FirebaseUser> OnLoginSuccess;
        public event Action<string> OnLoginFailed;
        public event Action OnLogout;

        #region Google 로그인 기능

        /// <summary>
        /// 구글 로그인을 하기위해서 파이어베이스와 연동하기 위한 초기화 단계
        /// </summary>
        public void InitGoogleSignIn()
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration()
            {
                WebClientId = webClientID,
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestEmail = true
            };
        }

        /// <summary>
        /// 구글 로그인 기능
        /// </summary>
        public async void SignInWithGoogle()
        {
            if (GoogleSignIn.Configuration == null)
            {
                InitGoogleSignIn();
            }

            try
            {
                var googleUser = await GoogleSignIn.DefaultInstance.SignIn();
                var credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
                var authResult = await Firebase.Auth.SignInWithCredentialAsync(credential);

                Firebase.User = authResult;

                OnLoginSuccess?.Invoke(Firebase.User);
            }
            catch (Exception ex)
            {
                OnLoginFailed?.Invoke(ex.Message);
                Debug.LogError($"Google Sign-In Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// 로그인한 계정을 로그아웃하는 기능
        /// </summary>
        public void SignOut()
        {
# if !UNITY_EDITOR
            GoogleSignIn.DefaultInstance.SignOut();
# endif
            Firebase.Auth.SignOut();
            Firebase.User = null;

            OnLogout?.Invoke();
        }

        /// <summary>
        /// 익명으로 로그인 하는 기능
        /// </summary>
        public async void SignInAnonymously()
        {
            try
            {
                var authResult = await Firebase.Auth.SignInAnonymouslyAsync();
                Firebase.User = authResult.User;
                OnLoginSuccess?.Invoke(Firebase.User);
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke(e.Message);
                Debug.LogError($"Anonymous Sign-In Exception: {e.Message}");
            }
        }

        /// <summary>
        /// 익명으로 로그인한 계정을 구글과 연동하는 기능
        /// </summary>
        public async Task<bool> LinkGoogleAccountAsync()
        {
            if (Firebase.Auth.CurrentUser == null || !Firebase.Auth.CurrentUser.IsAnonymous)
            {
                OnLoginFailed?.Invoke("현재 로그인한 익명의 사용자가 없습니다.");
                Debug.LogError("LinkGoogleAccount Error: 익명 사용자가 로그인하지 않았습니다.");
                return false;
            }

            if (GoogleSignIn.Configuration == null)
            {
                InitGoogleSignIn();
            }

            try
            {
                GoogleSignIn.DefaultInstance.SignOut();
                await Task.Delay(100);

                var googleUser = await GoogleSignIn.DefaultInstance.SignIn();
                var credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
                var authResult = await Firebase.Auth.CurrentUser.LinkWithCredentialAsync(credential);

                Firebase.User = authResult.User;
                OnLoginSuccess?.Invoke(Firebase.User);

                return true;
            }
            catch (FirebaseException ex)
            {
                // Google 계정이 이미 다른 Firebase 계정에 연결되어 있을 때 발생.
                // 사용자의 실수로 인하여 이미 다른 Firebase 계정에 연동되어있을 경우가 있기 때문에 생성
                if (ex.ErrorCode == (int)AuthError.CredentialAlreadyInUse)
                {
                    OnLoginFailed?.Invoke("이 Google 계정은 이미 다른 사용자와 연결되어있습니다.");
                    Debug.LogError("Google Link Exception: CredentialAlreadyInUse");
                }
                else
                {
                    OnLoginFailed?.Invoke("이 Google 계정은 이미 다른 사용자와 연결되어있습니다.");
                    Debug.LogError($"Google Link Exception: {ex.Message}");
                }

                return false;
            }
            catch (Exception ex)
            {
                OnLoginFailed?.Invoke("이 Google 계정은 이미 다른 사용자와 연결되어있습니다.");
                Debug.LogError($"Google Link Exception: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}