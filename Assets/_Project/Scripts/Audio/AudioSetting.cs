using CocoDoogy.Core;
using System;
using UnityEngine;

namespace CocoDoogy.Audio
{
    public class AudioSetting : Singleton<AudioSetting>
    {
        public static event Action<float> OnMasterChanged = null;
        public static event Action<float> OnBgmChanged = null;
        public static event Action<float> OnSfxChanged = null;

        public static event Action<bool> OnMasterMuteChanged = null;
        public static event Action<bool> OnBgmMuteChanged = null;
        public static event Action<bool> OnSfxMuteChanged = null;
        
        
        public static float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = value;

                OnMasterChanged?.Invoke(masterVolume);
            }
        }
        public static float BgmVolume
        {
            get => bgmVolume;
            set
            {
                bgmVolume = value;

                OnBgmChanged?.Invoke(bgmVolume);
            }
        }
        public static float SfxVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = value;

                OnSfxChanged?.Invoke(sfxVolume);
            }
        }
        
        public static bool IsMasterMuted
        {
            get => isMasterMuted;
            set
            {
                isMasterMuted = value;
                OnMasterMuteChanged?.Invoke(isMasterMuted);
            }
        }
        public static bool IsBgmMuted
        {
            get => isBgmMuted;
            set
            {
                isBgmMuted = value;
                OnBgmMuteChanged?.Invoke(isBgmMuted);
            }
        }
        public static bool IsSfxMuted
        {
            get => isSfxMuted;
            set
            {
                isSfxMuted = value;
                OnSfxMuteChanged?.Invoke(isSfxMuted);
            }
        }
        
        private static bool isMasterMuted = false;
        private static bool isBgmMuted = false;
        private static bool isSfxMuted = false;
        
        //볼륨
        private static float masterVolume = 0.75f;
        private static float bgmVolume = 0.75f;
        private static float sfxVolume = 0.75f;
        
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            DontDestroyOnLoad(gameObject);

            //VolumeController.OnVolumeChanged += AwakeAudioSetting;
        }

        private void Start()
        {
            AwakeAudioSetting();
        }

        private static void OnMasterSave(float value)
        {
            PlayerPrefs.SetFloat("MasterVolume", value);
            VolumeController.Instance.SetMasterVolume(value);
        }
        private static void OnBgmSave(float value)
        {
            PlayerPrefs.SetFloat("BgmVolume", value);
            VolumeController.Instance.SetBgmVolume(value);
        }
        private static void OnSfxSave(float value)
        {
            PlayerPrefs.SetFloat("SfxVolume", value);
            VolumeController.Instance.SetSfxVolume(value);
        }

        private static void OnMasterMuteSave(bool isMuted)
        {
            PlayerPrefs.SetInt("IsMasterMuted", isMuted ? 1 : 0);
            // Assuming VolumeController will have this method
            VolumeController.Instance.SetMasterMute(isMuted);
        }
        private static void OnBgmMuteSave(bool isMuted)
        {
            PlayerPrefs.SetInt("IsBgmMuted", isMuted ? 1 : 0);
            VolumeController.Instance.SetBgmMute(isMuted);
        }
        private static void OnSfxMuteSave(bool isMuted)
        {
            PlayerPrefs.SetInt("IsSfxMuted", isMuted ? 1 : 0);
            VolumeController.Instance.SetSfxMute(isMuted);
        }
        
        private void AwakeAudioSetting()
        {
            OnMasterChanged += OnMasterSave;
            OnBgmChanged += OnBgmSave;
            OnSfxChanged += OnSfxSave;

            OnMasterMuteChanged += OnMasterMuteSave;
            OnBgmMuteChanged += OnBgmMuteSave;
            OnSfxMuteChanged += OnSfxMuteSave;
            
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
            BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.75f);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
            
            IsMasterMuted = PlayerPrefs.GetInt("IsMasterMuted", 0) == 1;
            IsBgmMuted = PlayerPrefs.GetInt("IsBgmMuted", 0) == 1;
            IsSfxMuted = PlayerPrefs.GetInt("IsSfxMuted", 0) == 1;
        }
    }
}