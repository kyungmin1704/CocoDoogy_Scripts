using CocoDoogy.Core;
using FMOD.Studio;
using FMODUnity;

namespace CocoDoogy.Audio
{
    public class VolumeController : Singleton<VolumeController>
    {
        private const string VcaMasterPath = "vca:/Master";
        private const string VcaSfxPath = "vca:/Sfx";
        private const string VcaBgmPath = "vca:/Bgm";

        private VCA vcaMaster;
        private VCA vcaSfx;
        private VCA vcaBgm;

        //public static event Action OnVolumeChanged = null;
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            DontDestroyOnLoad(gameObject);

            vcaMaster = RuntimeManager.GetVCA(VcaMasterPath);
            vcaSfx = RuntimeManager.GetVCA(VcaSfxPath);
            vcaBgm = RuntimeManager.GetVCA(VcaBgmPath);
            
            //OnVolumeChanged?.Invoke();
        }

        public void SetMasterVolume(float volume)
        {
            vcaMaster.setVolume(AudioSetting.IsMasterMuted ? 0f : volume);
        }

        public void SetSfxVolume(float volume)
        {
            vcaSfx.setVolume(AudioSetting.IsSfxMuted ? 0f : volume);
        }

        public void SetBgmVolume(float volume)
        {
            vcaBgm.setVolume(AudioSetting.IsBgmMuted ? 0f : volume);
        }
        
        public void SetMasterMute(bool isMuted)
        {
            SetMasterVolume(AudioSetting.MasterVolume);
        }

        public void SetSfxMute(bool isMuted)
        {
            SetSfxVolume(AudioSetting.SfxVolume);
        }

        public void SetBgmMute(bool isMuted)
        {
            SetBgmVolume(AudioSetting.BgmVolume);
        }
    }
}
