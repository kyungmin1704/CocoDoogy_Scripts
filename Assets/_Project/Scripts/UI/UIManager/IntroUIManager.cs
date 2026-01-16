using CocoDoogy.UI.IntroAndLogin;
using CocoDoogy.Core;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.UIManager
{
    public class IntroUIManager : Singleton<IntroUIManager>
    {
        [SerializeField] private IntroUI introUI;
        [SerializeField] private LoginUI loginUI;
        [SerializeField] private RegisterUI registerUI;

        public IntroUI IntroUI => introUI;
        public LoginUI LoginUI => loginUI;
        public RegisterUI RegisterUI => registerUI;

        /// <summary>
        /// 닉네임 입력 팝업 띄우는 메서드 (RegisterUI)
        /// </summary>
        /// <returns></returns>
        public Task<string> ShowNicknameInputPopupAsync()
        {
            return registerUI.InputNicknameAsync();
        }

        /// <summary>
        /// 닉네임 입력 후 예외조건을 통과하지 못하면 에러 메세지를 띄우는 메서드
        /// </summary>
        /// <param name="errorMessage"></param>
        public async Task ShowErrorPopupAsync(string errorMessage)
        {
            await registerUI.ErrorPopup.ShowPopupAsync(errorMessage, 1, 0.5f);
        }

        /// <summary>
        /// 닉네임 입력 후 예외조건을 통과한 다음에 메세지를 띄우는 메서드
        /// </summary>
        /// <param name="message"></param>
        public async Task ShowCreatePopupAsync(string message)
        {
            await registerUI.CreatePopup.ShowPopupAsync(message, 1, 0.5f);
        }
    }
}
