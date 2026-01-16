using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.UI.Replay;
using CocoDoogy.UI.UserInfo;
using UnityEngine;

namespace CocoDoogy.UI.UIManager
{
    public class ReplayUIManager : Singleton<ReplayUIManager>
    {
        public static double timer;
        public static string consumeAP;
        public static string refillCount;
        [Header("Buttons")]
        [SerializeField] private CommonButton openSettingsButton;
        [SerializeField] private CommonButton openPauseButton;
        
        [Header("UI Panel")]
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private ReplayPauseUI replayPauseUI;
        [SerializeField] private ReplayTimer replayTimerUI;
        [SerializeField] private ReplayConsole console;
        protected override void Awake()
        {
            base.Awake();
            openSettingsButton.onClick.AddListener(OnClickOpenSetting);
            openPauseButton.onClick.AddListener(OnClickOpenPause);
            replayTimerUI.SetTimer(timer);
        }

        private void OnClickOpenSetting()
        {
            SfxManager.PlayDucking(0.7f);
            settingsUI.OpenPanel();
        }

        private void OnClickOpenPause()
        {
            replayPauseUI.OpenUI();
        }
        
    }
}