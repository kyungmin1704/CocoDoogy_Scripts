using CocoDoogy.Data;
using CocoDoogy.UI.UIManager;
using Firebase.Auth;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CocoDoogy.Network.Login
{
    public class LoginViewModel
    {
        private readonly AuthProvider authProvider;

        public FirebaseUser CurrentUser { get; private set; }
        public string ErrorMessage { get; private set; }

        public event Action<FirebaseUser> OnUserChanged;
        public event Action<string> OnErrorChanged;
        public event Action OnLoggedOut;

        public LoginViewModel(AuthProvider provider)
        {
            authProvider = provider;
            authProvider.OnLoginSuccess += HandleLoginSuccess;
            authProvider.OnLoginFailed += HandleLoginFailed;
            authProvider.OnLogout += HandleLogout;
        }

        #region < 로그인 관련 기능 >
        private async void HandleLoginSuccess(FirebaseUser user)
        {
            bool isExistingUser = await UserData.CheckUserProfileExistsAsync(user.UserId);

            if (!isExistingUser) // 기존 유저가 아닌 경우
            {
                string nickname = await GetNickNameAsync(user.UserId);
                await UserData.CreateOnServerAsync(user.UserId, nickname);
            }

            CurrentUser = user;
            DataManager.Instance.StartListeningForUserData(CurrentUser.UserId);
            
            FirebaseManager.Instance.AuthStateChanged();
            
            OnUserChanged?.Invoke(user);
        }
        private void HandleLoginFailed(string error)
        {
            ErrorMessage = error;
            OnErrorChanged?.Invoke(error);
        }
        private void HandleLogout()
        {
            CurrentUser = null;

            FirebaseManager.Instance.AuthStateChanged();
            
            OnLoggedOut?.Invoke();
        }
        public void SignIn() => authProvider.SignInWithGoogle();
        public void SignOut() => authProvider.SignOut();
        #endregion


        #region < 익명로그인 기능 & 익명로그인 링크 구글 기능 > 
        public void SignInAnonymously() => authProvider.SignInAnonymously();

        public async Task<bool> LinkGoogleAccountAsync()
        {
            return await authProvider.LinkGoogleAccountAsync();
        }

        #endregion

        #region < 로그인 이후 최초 가입 시 닉네임 입력 시퀀스 >

        private async Task<string> GetNickNameAsync(string uid)
        {
            string nickname = null;
            bool isSuccess = false;

            while (!isSuccess)
            {
                nickname = await IntroUIManager.Instance.ShowNicknameInputPopupAsync();

                if (!CanUse(nickname)) continue;

                try
                {
                    bool isAvailable = await UserData.TrySetNewNickNameAsync(uid, nickname);

                    if (isAvailable)
                    {
                        await IntroUIManager.Instance.ShowCreatePopupAsync("닉네임 생성이 완료되었습니다.");
                        isSuccess = true;
                    }
                    else
                    {
                        await IntroUIManager.Instance.ShowErrorPopupAsync("이미 존재하는 닉네임 입니다.");
                    }
                }
                catch (Exception ex)
                {
                    await IntroUIManager.Instance.ShowErrorPopupAsync($"오류!\n닉네임 검사 중 오류가 발생했습니다: {ex.Message}.");
                }
            }
            return nickname;
        }

        #endregion


        #region < 닉네임 입력 확인 연쇄책임패턴 > 
        /// <summary>
        /// 입력된 닉네임이 적합한 닉네임인지 확인
        /// </summary>
        private bool CanUse(string nickname)
        {
            foreach (var check in CanUseNickname)
            {
                if (!check(nickname)) return false;
            }
            return true;
        }

        /// <summary>
        /// 현재 입력된 닉네임이 예외 조건에 걸리지 않는지 확인하는 책임연쇄패턴
        /// </summary>
        private static readonly Func<string, bool>[] CanUseNickname =
        {
            IsEmpty,
            IsKoreanAlphaNickname,
            IsKoreanNickname,
            IsEnglishNickname,
            IsMixedNickname,
            IsSpecialSymbol,
        };

        /// <summary>
        /// 현재 입력된 닉네임이 비어있는가
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private static bool IsEmpty(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                _ = IntroUIManager.Instance.ShowErrorPopupAsync("닉네임은 비워둘 수 없습니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 한글의 자음 혹은 모음이 닉네임에 포함되어있는가
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private static bool IsKoreanAlphaNickname(string nickname)
        {
            if (Regex.IsMatch(nickname, @"[\u1100-\u11FF\u3130-\u318F\uFFA0-\uFFDC]"))
            {
                _ = IntroUIManager.Instance.ShowErrorPopupAsync("자음 또는 모음 단독 입력은 허용되지 않습니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 한글로 되어있는 닉네임이 최소 2글자 최대 6글자 사이로 이루어져있는가
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private static bool IsKoreanNickname(string nickname)
        {
            if (Regex.IsMatch(nickname, @"^[가-힣0-9]+$") && nickname.Length is < 2 or > 6)
            {
                _ = IntroUIManager.Instance.ShowErrorPopupAsync("한글 닉네임은 최소 2글자 최대 6글자 사이로 입력가능합니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 영어로된 닉네임이 최소 4글자 최대 12글자 사이로 이루어져있는가
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private static bool IsEnglishNickname(string nickname)
        {
            if (Regex.IsMatch(nickname, @"^[a-zA-Z0-9]+$") && nickname.Length is < 4 or > 12)
            {
                _ = IntroUIManager.Instance.ShowErrorPopupAsync("영어 닉네임은 최소 4글자 최대 12글자 사이로 입력가능합니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 한글 + 영어 닉네임이 최소 4글자 최대 12글자 사이로 이루어져있는가
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private static bool IsMixedNickname(string nickname)
        {
            if (Regex.IsMatch(nickname, @"^(?=.*[가-힣])(?=.*[A-Za-z])[A-Za-z가-힣0-9]+$") && nickname.Length is < 4 or > 12)
            {
                _ = IntroUIManager.Instance.ShowErrorPopupAsync("혼합 닉네임은 최소 4글자 최대 12글자 사이로 입력가능합니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 특수기호가 사용되어있는가
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private static bool IsSpecialSymbol(string nickname)
        {
            if (Regex.IsMatch(nickname, @"[^\w\.@-]"))
            {
                _ = IntroUIManager.Instance.ShowErrorPopupAsync("특수 기호 입력은 허용되지 않습니다.");
                return false;
            }
            return true;
        }
        #endregion
    }
}