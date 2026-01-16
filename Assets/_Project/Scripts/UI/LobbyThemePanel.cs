using CocoDoogy.CameraSwiper;
using CocoDoogy.Core;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI
{
    public class LobbyThemePanel: MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI text;


        private CancellationTokenSource token = null;
        private Theme currentTheme = Theme.None;


        void Awake()
        {
            PageCameraSwiper.OnEndPageChanged += OnThemeChanged;
        }
        void OnDestroy()
        {
            PageCameraSwiper.OnEndPageChanged -= OnThemeChanged;
            Stop();
        }


        private void OnThemeChanged(Theme theme)
        {
            if (currentTheme == theme) return;
            
            Stop();

            text.text = (currentTheme = theme).ToName();
            _ = ChangeAlphaAsync();
        }

        private void Stop()
        {
            token?.Cancel();
            token = null;
        }

        private async UniTask ChangeAlphaAsync()
        {
            token = new();

            canvasGroup.alpha = 0.5f;
            while(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime;
                await UniTask.Yield(token.Token);
            }
            canvasGroup.alpha = 1f;
                await UniTask.WaitForSeconds(1f, cancellationToken: token.Token);
            while(canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime;
                await UniTask.Yield(token.Token);
            }
            canvasGroup.alpha = 0f;

            token = null;
        }
    }
}