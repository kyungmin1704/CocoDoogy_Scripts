using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.Replay;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.Utility.Loading;
using System;
using UnityEngine;

namespace CocoDoogy.UI.Replay
{
    public class ReplayPauseUI : MonoBehaviour
    {
        [SerializeField] private CommonButton resumeButton;
        [SerializeField] private CommonButton restartButton;
        [SerializeField] private CommonButton quitButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnClickResume);
            restartButton.onClick.AddListener(OnClickRestart);
            quitButton.onClick.AddListener(OnClickQuit);
        }

        private void OnEnable()
        {
            InGameManager.Timer.Pause();
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
            InGameManager.Timer.Start();
        }

        public void OpenUI()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 계속 버튼
        /// </summary>
        private void OnClickResume()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 재시작 버튼
        /// </summary>
        private void OnClickRestart()
        {
            MessageDialog.ShowMessage("다시하기", "모든 걸 버리고 다시 시작할까요?", DialogMode.YesNo, ResetOrNot);
        }

        /// <summary>
        /// 리플레이 나가기 버튼
        /// </summary>
        private void OnClickQuit()
        {
            MessageDialog.ShowMessage("나가기", "주인은 다음에 찾을까요?", DialogMode.YesNo, QuitOrNot);
        }
        
        private void ResetOrNot(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                SfxManager.StopDucking();
                Loading.LoadScene("Replay");
            }
        }

        private void QuitOrNot(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                SfxManager.StopDucking();
                Loading.LoadScene("Lobby");
            }
        }
    }
}