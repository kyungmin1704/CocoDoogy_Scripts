using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.IntroAndLogin
{
    public class MessagePopup : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI messageText;

        /// <summary>
        /// 메시지 팝업 전체를 보여주고 점점 사라지게 함
        /// </summary>
        public async Task ShowPopupAsync(string message, float duration = 1f, float fadeDuration = 0.5f)
        {
            gameObject.SetActive(true);
            messageText.text = message;

            SetAlpha(1f);

            var tcs = new TaskCompletionSource<bool>();

            DOTween.To(() => 1f, SetAlpha, 0f, fadeDuration)
                .SetDelay(duration)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    messageText.text = string.Empty;
                    tcs.SetResult(true);
                });

            await tcs.Task;
        }

        /// <summary>
        /// 텍스트와 배경 알파값 설정
        /// </summary>
        private void SetAlpha(float alpha)
        {
            if (messageText != null)
            {
                Color txtColor = messageText.color;
                messageText.color = new Color(txtColor.r, txtColor.g, txtColor.b, alpha);
            }

            if (backgroundImage != null)
            {
                Color bgColor = backgroundImage.color;
                backgroundImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);
            }
        }

        /// <summary>
        /// 즉시 팝업 숨기기
        /// </summary>
        public void HidePopup()
        {
            DOTween.Kill(messageText);
            DOTween.Kill(backgroundImage);
            gameObject.SetActive(false);
            messageText.text = string.Empty;
        }
    }
}