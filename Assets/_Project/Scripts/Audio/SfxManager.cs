using CocoDoogy.Core;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Audio
{
    public class SfxManager : Singleton<SfxManager>
    {
        //모든 Sfx류의 사운드를 여기에서 관리합니다.
        private static bool HasInstance => Instance != null;
        
        [Header("SFXListData")]
        public SfxListData sfxListData;
        
        //런타임에서 빠른 검색 위해서 만든 딕셔너리
        private Dictionary<SfxType, EventInstance> sfxDictionary = new ();
        
        //효과음 재생간 브금 감소를 위한 변수들 (Ducking)
        public EventReference duckingSnapShot;
        private EventInstance duckingInstance;
        public EventReference mutingSnapShot;
        private EventInstance mutingInstance;
        
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            
            DontDestroyOnLoad(gameObject);
            
            InitializeDictionary();
            duckingInstance = RuntimeManager.CreateInstance(duckingSnapShot);
            mutingInstance = RuntimeManager.CreateInstance(mutingSnapShot);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var sfxType in sfxDictionary)
            {
                sfxType.Value.release();
            }
            
            duckingInstance.release();
            mutingInstance.release();
        }
        
        /// <summary>
        /// 정해진 효과음을 재생합니다.
        /// </summary>
        /// <param name="sfxType"></param>
        public static void PlaySfx(SfxType sfxType)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("PlaySfx : 인스턴스가 존재하지 않습니다!");
                return;
            }

            if (!Instance.sfxDictionary.TryGetValue(sfxType, out EventInstance eventInstance))
            {
                return;
            }

            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.start();
        }
        
        private static void InitializeDictionary()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("InitializeDictionary : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            foreach (var sfxType in Instance.sfxListData.sfxList)
            {
                EventInstance sfxInstance = RuntimeManager.CreateInstance(sfxType.eventReference);
                Instance.sfxDictionary.Add(sfxType.type, sfxInstance);
                
                //출력 테스트 라인 with BBUX
                /*sfxDictionary[sfxType.type].getDescription(out var eventDescription);
                eventDescription.getPath(out var path);
                eventDescription.getID(out var id);
                Debug.Log(path + " " + id);*/
            }
        }
        
        public static void ToggleLoopSound(SfxType sfxType)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("ToggleLoopSound : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            if (sfxType != SfxType.Loop_Detecting && sfxType != SfxType.Loop_ShakeUmbrella)
            {
                return;
            }
            
            Instance.sfxDictionary[sfxType].getPlaybackState(out PLAYBACK_STATE state);
            
            if (state == PLAYBACK_STATE.STOPPED)
            {
                Instance.sfxDictionary[sfxType].start();
            }
            else
            {
                Instance.sfxDictionary[sfxType].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
        
        //
        
        public static void InitDetectingLevelParameter(int level)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("InitDetectingLevelParameter : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            level = Mathf.Clamp(level, 1, 3);

            if (Instance.sfxDictionary.TryGetValue(SfxType.Loop_Detecting, out EventInstance eventInstance))
            {
                Instance.sfxDictionary[SfxType.Loop_Detecting].setParameterByName("Level", level);
            }
        }

        #region Ducking & Muting
        //Ducking이란? 특정 상황에서 BGM을 줄여서 몰입도를 늘리는 기능입니다.
        //Ducking이 필요하면 이 메서드를 PlaySfx 메서드를 불러오기 전에 같이 불러오고, 종료시 StopDucking 불러오면 됩니다.
        //FMOD의 심각한 결함으로 어쩔 수 없이 두가지 기능을 나누었습니다. (이중 오토메이션 구현시 프로그램 다운 현상)
        public static void PlayDucking(float duckingVolume = 0.7f)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("PlayDucking : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            duckingVolume = Mathf.Clamp(duckingVolume, 0, 1);
            
            Instance.duckingInstance.setParameterByName("DuckingVolume", duckingVolume);
            Instance.duckingInstance.start();
        }

        public static void StopDucking()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("StopDucking : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            Instance.duckingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public static void PlayMuting()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("PlayMuting : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            Instance.mutingInstance.setParameterByName("MutingVolume", 0f);
            Instance.mutingInstance.start();
        }

        public static void StopMuting()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("PlayMuting : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            Instance.mutingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        #endregion
        
        public static void StopSfx(SfxType sfxType)
        {
            if (!HasInstance) return;
            
            if (!Instance.sfxDictionary.TryGetValue(sfxType, out EventInstance eventInstance)) return;

            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
