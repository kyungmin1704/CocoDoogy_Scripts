using CocoDoogy.Core;
using CocoDoogy.WeatherEffect;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoDoogy.Audio
{
    public class BgmManager : Singleton<BgmManager>
    {
        //모든 Bgm류의 사운드를 여기에서 관리합니다.
        private static bool HasInstance => Instance != null;
        
        [Header("BGMList")]
        public List<BgmReference> bgmList = new();

        [Header("Bgm Setting By theme")] 
        [SerializeField] private List<ThemeBgm> themeBgmSettings;
        
        private Dictionary<Theme, BgmType> themeBgmDict = new();
        private BgmType nextBgmToPlay = BgmType.LobbyBgm; //Bgm 캐싱
        
        //런타임에서 빠른 검색을 위해서 만든 딕셔너리
        private Dictionary<BgmType, EventInstance> bgmDictionary = new();
        
        //현재 재생중인 Bgm
        private BgmType currentBgmType = BgmType.None;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (Instance != this) return;
            DontDestroyOnLoad(gameObject);
            
            //초기화 작업
            InitThemeBgmDict();
            InitBgmDictionary();
            InitSceneManager();
            
            PlayBgm(BgmType.LobbyBgm);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var bgmType in bgmDictionary)
            {
                bgmType.Value.release();
            }
        }
        
        /// <summary>
        /// BgmType에 맞는 Bgm만 틉니다.
        /// </summary>
        /// <param name="bgmType"></param>
        public static void PlayBgm(BgmType bgmType)
        {
            if (!Instance)
            {
                Debug.LogWarning("ToggleBgm : 인스턴스가 존재하지 않습니다!");
                return;
            }

            if (Instance.currentBgmType == bgmType)
            {
                return;
            }
            
            MuteBgm();
            
            if (Instance.bgmDictionary.TryGetValue(bgmType, out EventInstance eventInstance))
            {
                eventInstance.start();
                Instance.currentBgmType = bgmType;
            }
        }
        
        /// <summary>
        /// 모든 배경음악을 중단합니다.
        /// </summary>
        /// <param name="fadeOut">true면 fadeout 아니면 false면 즉시종료</param>
        public static void MuteBgm(bool fadeOut = true)
        {
            if (!Instance) return;
            
            foreach (var BgmType in Instance.bgmDictionary)
            {
                if (fadeOut)
                {
                    BgmType.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                else
                {
                    BgmType.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                }
            }
        }

        public static void PrepareStageBgm(Theme theme)
        {
            if (!Instance) return;

            if (Instance.themeBgmDict.TryGetValue(theme, out var bgmType))
            {
                Instance.nextBgmToPlay = bgmType;
            }
            else
            {
                Instance.nextBgmToPlay = BgmType.LobbyBgm;
            }
        }
        
        #region Init
        //theme와 bgmType이 서로 다른 Enum을 쓰기 때문에 매칭했습니다.
        private void InitThemeBgmDict()
        {
            foreach (var themeBgm in themeBgmSettings)
            {
                if (!themeBgmDict.ContainsKey(themeBgm.theme) && themeBgm.theme != Theme.None)
                {
                    themeBgmDict.Add(themeBgm.theme, themeBgm.bgmType);
                }
            }
        }
        
        //BgmType과 EventInstance를 매칭 시켜 줍니다.
        private void InitBgmDictionary()
        {
            if (!Instance)
            {
                Debug.LogWarning("InitializeDictionary : 인스턴스가 존재하지 않습니다!");
                return;
            }

            foreach (var bgmType in Instance.bgmList)
            {
                EventInstance bgmInstance = RuntimeManager.CreateInstance(bgmType.eventReference);
                Instance.bgmDictionary.Add(bgmType.type, bgmInstance);
            }
        }

        //여기서 Scene이 바뀔때 bgm도 바뀌도록 이벤트에 구독합니다.
        private void InitSceneManager()
        {
            //SceneManager의 이벤트를 구독 (메서드는 따로 만들어야함)
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Loading")
                {
                    SfxManager.PlayMuting();
                    SfxManager.StopSfx(SfxType.Weather_Rain); //지속성 이벤트는 이것하나라서 직접 불렀습니다.
                }
                
                if (scene.name == "InGame" && Instance.nextBgmToPlay != BgmType.None)
                {
                    PlayBgm(Instance.nextBgmToPlay);
                    Instance.nextBgmToPlay = BgmType.None;
                }
                
                if (scene.name == "Lobby")
                {
                    PlayBgm(BgmType.LobbyBgm);
                }
            };
            
            SceneManager.sceneUnloaded += (scene) =>
            {
                if (scene.name == "Loading")
                {
                    SfxManager.StopMuting();
                }
            };
        }
        #endregion
    }
}
