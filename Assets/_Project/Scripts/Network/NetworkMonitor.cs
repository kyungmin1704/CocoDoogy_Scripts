using CocoDoogy.Core;
using CocoDoogy.UI.Popup;
using CocoDoogy.Utility.Loading;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CocoDoogy.Network
{
    public class NetworkMonitor : Singleton<NetworkMonitor>
    {
        public bool IsConnected { get; private set; } = true;
        public event Action<bool> OnConnectionChanged;

        private bool lastState = true;
        private bool isChecking = false;

        private MessageDialog popup;
        private FirebaseLoading loadingUI;

        private const string PingURL = "https://google.com";

        protected override void Awake()
        {
            base.Awake();
            InvokeRepeating(nameof(CheckNetwork), 0f, 2f);
        }

        private async UniTaskVoid CheckNetwork()
        {
            if (isChecking) return;
            isChecking = true;

            bool nowConnected = await IsInternetAvailable();
       
            if (nowConnected == lastState)
            {
                isChecking = false;
                return;
            }

            lastState = nowConnected;
            IsConnected = nowConnected;
            OnConnectionChanged?.Invoke(IsConnected);

            if (!nowConnected)
            {
                Debug.Log("인터넷 끊김... 재연결 시도...");

                if (!SceneReady())
                {
                    isChecking = false;
                    return;
                }

                loadingUI = FirebaseLoading.ShowLoading();

                bool reconnected = await WaitForReconnect(10f);

                loadingUI?.Hide();

                if (reconnected)
                {
                    Debug.Log("재연결 성공");
                }
                else
                {
                    Debug.Log("재연결 실패");

                    if (!popup)
                    {
                        popup = MessageDialog.ShowMessage(
                            "인터넷 연결 실패",
                            "인터넷 연결이 끊겼습니다.\n인터넷 상태를 확인해주세요.",
                            DialogMode.Confirm,
                            OnPopupClosed
                        );
                    }
                }
            }
            else
            {
                Debug.Log("인터넷 연결됨");
            }

            isChecking = false;
        }

        /// <summary>
        /// 실제 인터넷이 가능한지 서버 Ping으로 체크
        /// </summary>
        private async UniTask<bool> IsInternetAvailable()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;

            try
            {
                using (var request = UnityWebRequest.Get(PingURL))
                {
                    request.timeout = 3;
                    await request.SendWebRequest();

                    return request.result == UnityWebRequest.Result.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 10초 동안 재연결 시도
        /// </summary>
        private async UniTask<bool> WaitForReconnect(float maxSeconds)
        {
            float timer = 0;

            while (timer < maxSeconds)
            {
                if (await IsInternetAvailable())
                {
                    IsConnected = true;
                    lastState = true;
                    return true;
                }

                await UniTask.Delay(500);
                timer += 0.5f;
            }

            return false;
        }

        /// <summary>
        /// 씬 로딩/전환 중인지 체크하여 UI 중복 방지
        /// </summary>
        private bool SceneReady()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.isLoaded) return false;

            return true;
        }

        /// <summary>
        /// 팝업 닫을 때 Intro 재진입
        /// </summary>
        private void OnPopupClosed(CallbackType callback)
        {
            if (callback == CallbackType.Yes)
            {
                popup = null;
                Loading.LoadScene("Intro");
            }
        }
    }
}