using CocoDoogy.Audio;
using CocoDoogy.CameraSwiper;
using CocoDoogy.GameFlow.InGame;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.UserInfo
{
    public class SettingsUI : UIPanel
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform settingsWindow;
        
        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        
        [Header("Volume Icons")]
        [SerializeField] private Image masterIcon;
        [SerializeField] private Image bgmIcon;
        [SerializeField] private Image sfxIcon;
        [SerializeField] private Image sfxInnerIcon;
        
        [Header("Volume UI Sliders")]
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider bgmVolume;
        [SerializeField] private Slider sfxVolume;

        [Header("Mute")]
        [SerializeField] private Button masterMute;
        [SerializeField] private Button bgmMute;
        [SerializeField] private Button sfxMute;
        [SerializeField] private Image masterSlider;
        [SerializeField] private Image bgmSlider;
        [SerializeField] private Image sfxSlider;

        private bool _isUpdatingState = false; // 동시 변경 방지 플래그

        private const float Color_Disabled = 0.5f;
        private const float Color_Enabled = 1.0f;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);

            // 슬라이더 리스너
            masterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
            bgmVolume.onValueChanged.AddListener(OnBgmVolumeChanged);
            sfxVolume.onValueChanged.AddListener(OnSfxVolumeChanged);

            // 뮤트 버튼 리스너
            masterMute.onClick.AddListener(() => OnMasterMuteClicked());
            bgmMute.onClick.AddListener(() => OnBgmMuteClicked());
            sfxMute.onClick.AddListener(() => OnSfxMuteClicked());
        }

        private void Start()
        {
            UpdateAllVisuals();
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
            if(InGameManager.Timer != null) InGameManager.Timer.Pause();
            
            masterVolume.SetValueWithoutNotify(AudioSetting.MasterVolume);
            bgmVolume.SetValueWithoutNotify(AudioSetting.BgmVolume);
            sfxVolume.SetValueWithoutNotify(AudioSetting.SfxVolume);
        }

        private void OnDisable()
        {
            if(InGameManager.Timer != null) InGameManager.Timer.Start();
            Time.timeScale = 1;
        }

        public override void ClosePanel()
        {
            settingsWindow.gameObject.SetActive(false);
            PageCameraSwiper.IsSwipeable = true;
        }

        #region Slider Event Handlers
        private void OnMasterVolumeChanged(float value)
        {
            if (_isUpdatingState) return;
            _isUpdatingState = true;

            AudioSetting.MasterVolume = value;

            // 슬라이더가 0이 되면 뮤트 상태를 true로 설정
            if (value <= 0)
            {
                AudioSetting.IsMasterMuted = true;
            }
            else
            {
                // 슬라이더가 0보다 커지면 뮤트 상태를 false로 설정
                AudioSetting.IsMasterMuted = false;
            }
            UpdateAllVisuals();
            _isUpdatingState = false;
        }

        private void OnBgmVolumeChanged(float value)
        {
            if (_isUpdatingState) return;
            _isUpdatingState = true;

            AudioSetting.BgmVolume = value;

            if (value <= 0)
            {
                AudioSetting.IsBgmMuted = true;
            }
            else
            {
                AudioSetting.IsBgmMuted = false;
            }
            UpdateAllVisuals();
            _isUpdatingState = false;
        }

        private void OnSfxVolumeChanged(float value)
        {
            if (_isUpdatingState) return;
            _isUpdatingState = true;

            AudioSetting.SfxVolume = value;

            if (value <= 0)
            {
                AudioSetting.IsSfxMuted = true;
            }
            else
            {
                AudioSetting.IsSfxMuted = false;
            }
            UpdateAllVisuals();
            _isUpdatingState = false;
        }
        #endregion

        #region 뮤트 기능
        private void OnMasterMuteClicked()
        {
            if (_isUpdatingState) return;
            _isUpdatingState = true;

            bool newMuteState = !AudioSetting.IsMasterMuted;

            // 중앙 데이터 변경
            AudioSetting.IsMasterMuted = newMuteState;

            // 마스터 뮤트를 풀면 하위 뮤트도 모두 해제. 뮤트하면 하위도 모두 뮤트.
            AudioSetting.IsBgmMuted = newMuteState;
            AudioSetting.IsSfxMuted = newMuteState;

            UpdateAllVisuals();
            _isUpdatingState = false;
        }

        private void OnBgmMuteClicked()
        {
            if (_isUpdatingState) return;
            _isUpdatingState = true;

            bool newMuteState = !AudioSetting.IsBgmMuted;

            // BGM을 켜려는데 마스터가 꺼져있다면 마스터도 켠다.
            if (!newMuteState && AudioSetting.IsMasterMuted)
            {
                AudioSetting.IsMasterMuted = false;
            }

            // BGM 상태 변경
            AudioSetting.IsBgmMuted = newMuteState;

            // BGM과 SFX가 둘 다 꺼지면 마스터도 끈다.
            if (newMuteState && AudioSetting.IsSfxMuted)
            {
                AudioSetting.IsMasterMuted = true;
            }

            UpdateAllVisuals();
            _isUpdatingState = false;
        }

        private void OnSfxMuteClicked()
        {
            if (_isUpdatingState) return;
            _isUpdatingState = true;

            bool newMuteState = !AudioSetting.IsSfxMuted;

            // SFX를 켜려는데 마스터가 꺼져있다면 마스터도 켠다.
            if (!newMuteState && AudioSetting.IsMasterMuted)
            {
                AudioSetting.IsMasterMuted = false;
            }

            // SFX 상태 변경
            AudioSetting.IsSfxMuted = newMuteState;

            // BGM과 SFX가 둘 다 꺼지면 마스터도 끈다.
            if (newMuteState && AudioSetting.IsBgmMuted)
            {
                AudioSetting.IsMasterMuted = true;
            }

            UpdateAllVisuals();
            _isUpdatingState = false;
        }
        #endregion

        #region 비주얼 업데이트
        private void UpdateAllVisuals()
        {
            UpdateMasterVisual();
            UpdateBgmVisual();
            UpdateSfxVisual();
        }
        
        private void UpdateMasterVisual()
        {
            float targetValue = (AudioSetting.IsMasterMuted || masterVolume.value <= 0) ? Color_Disabled : Color_Enabled;
            ApplyColorToGroup(targetValue, masterIcon, new List<Image> { masterSlider, masterVolume.image });
        }

        private void UpdateBgmVisual()
        {
            float targetValue = (AudioSetting.IsMasterMuted || AudioSetting.IsBgmMuted || bgmVolume.value <= 0) ? Color_Disabled : Color_Enabled;
            ApplyColorToGroup(targetValue, bgmIcon, new List<Image> { bgmSlider, bgmVolume.image });
        }

        private void UpdateSfxVisual()
        {
            float targetValue = (AudioSetting.IsMasterMuted || AudioSetting.IsSfxMuted || sfxVolume.value <= 0) ? Color_Disabled : Color_Enabled;
            ApplyColorToGroup(targetValue, sfxIcon, new List<Image> { sfxSlider, sfxVolume.image, sfxInnerIcon });
        }

        private void ApplyColorToGroup(float alphaValue, Image icon, List<Image> images)
        {
            Color targetColor = new Color(alphaValue, alphaValue, alphaValue, 1f);
            float duration = 0.2f;

            if (icon != null) icon.DOColor(targetColor, duration).SetUpdate(true);
            
            foreach (var img in images)
            {
                if (img != null) img.DOColor(targetColor, duration).SetUpdate(true);
            }
        }
        #endregion
    }
}
