using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.UI.InGame;
using CocoDoogy.UI.UserInfo;
using UnityEngine;


namespace CocoDoogy.UI.UIManager
{
    public class InGameUIManager : Singleton<InGameUIManager>
    {
        [Header("Main Buttons")]
        [SerializeField] private CommonButton openSettingsButton;
        [SerializeField] private CommonButton openPauseButton;

        [Header("Option UI Elements")]
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private InGamePauseUI pauseUI;

        [SerializeField] private InteractButton interactButton;
        
        protected override void Awake()
        {
            base.Awake();
            openSettingsButton.onClick.AddListener(OnClickSetting);
            openPauseButton.onClick.AddListener(OnClickPause);
        }
        
        private void OnClickSetting()
        {
            settingsUI.OpenPanel();
        }

        private void OnClickPause()
        {
            pauseUI.OpenUI();
            SfxManager.PlayDucking();
        }

        public void OnInteractButtonActive()
        {
            interactButton.OnInteractChanged();
        }
    }
}
