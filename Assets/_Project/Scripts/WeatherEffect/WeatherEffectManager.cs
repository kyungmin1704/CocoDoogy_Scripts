using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Weather;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace CocoDoogy.WeatherEffect
{
    public enum EffectSystemType
    {
        ParticleSystem,
        VFXGraph,
        Mixed,
        None
    }
    
    [System.Serializable]
    public struct WeatherEffectInfo
    {
        public WeatherType weatherType;
        public GameObject effect;
        public float duration;
        public EffectSystemType effectSystemType;
        public SfxType sfxType;
    }
    
    public class WeatherEffectManager : Singleton<WeatherEffectManager>
    {
        private static bool HasInstance => Instance != null;

        [Header("날씨 이펙트 리스트")]
        public List<WeatherEffectInfo> weatherEffectList = new ();
        
        private Dictionary<WeatherType, WeatherEffectInfo> weatherEffectDictionary = new ();
        
        private Coroutine _currentEffectCoroutine;
        
        protected override void Awake()
        {
            base.Awake();

            InitWeatherEffectDictionary();
        }

        private void OnEnable()
        {
            WeatherManager.OnWeatherChanged += OnWeatherChanged;
        }
        
        private void OnDisable()
        {
            WeatherManager.OnWeatherChanged -= OnWeatherChanged;
        }

        private void OnWeatherChanged(WeatherType weatherType)
        {
            if (weatherEffectDictionary.TryGetValue(weatherType, out WeatherEffectInfo effectInfo))
            {
                SfxManager.StopSfx(weatherEffectDictionary[weatherType].sfxType);
                PlayEffect(weatherType, effectInfo.duration);
            }
        }
        
        private void InitWeatherEffectDictionary()
        {
            weatherEffectDictionary.Clear();
            
            foreach (var weatherEffectInfo in weatherEffectList)
            {
                weatherEffectDictionary.Add(weatherEffectInfo.weatherType, weatherEffectInfo);
                weatherEffectInfo.effect.SetActive(false);
            }
        }
        
        /// <summary>
        /// 이펙트를 키고 끕니다.
        /// </summary>
        /// <param name="weatherType"></param>
        /// <param name="duration"></param>
        public static void PlayEffect(WeatherType weatherType, float duration = 3f)
        {
            if (!HasInstance) return;

            if (duration <= 0)
            {
                print("WeatherEffectManager : 잘못된 값 입력으로 초기값 투입");
                duration = 3f;
            }
            
            Instance.PlayEffectInternal(weatherType, duration);
        }

        #region 내부 로직
        // 코루틴용 메서드
        private void PlayEffectInternal(WeatherType weatherType, float duration)
        {
            //키 값 확인
            if (!weatherEffectDictionary.ContainsKey(weatherType))
            {
                Debug.LogWarning($"이펙트를 찾을 수 없습니다. : {weatherType}");
                return;
            }
            
            //코루틴 중단
            if (_currentEffectCoroutine != null)
            {
                StopCoroutine(_currentEffectCoroutine);
            }
            SfxManager.PlaySfx(weatherEffectDictionary[weatherType].sfxType);
            // 여기서 코루틴 사용
            _currentEffectCoroutine = StartCoroutine(PlayEffectCoroutine(
                weatherEffectDictionary[weatherType], 
                duration));
            
        }
        
        private IEnumerator PlayEffectCoroutine(WeatherEffectInfo effectInfo, float duration)
        {
            // 초기화
            foreach (WeatherEffectInfo effect in weatherEffectDictionary.Values)
            {
                effect.effect.SetActive(false);
            }
            
            //실행
            effectInfo.effect.SetActive(true);
            yield return new WaitForSeconds(duration);

            yield return StartCoroutine(StopEffectCoroutine(effectInfo));

            _currentEffectCoroutine = null;
        }

        private IEnumerator StopEffectCoroutine(WeatherEffectInfo effectInfo)
        {
            GameObject effectObject = effectInfo.effect;
            
            switch (effectInfo.effectSystemType)
            {
                case EffectSystemType.ParticleSystem:
                    yield return StopParticleSystem(effectObject);
                    break;
                    
                case EffectSystemType.VFXGraph:
                    yield return StopVFXGraph(effectObject);
                    break;
                    
                case EffectSystemType.Mixed:
                    // 둘 다 처리
                    yield return StopParticleSystem(effectObject);
                    yield return StopVFXGraph(effectObject);
                    break;
                    
                case EffectSystemType.None:
                    //확장형 컨트롤러 스크립트로 리팩토링 가능성?
                    WaveDistortionController waveDistortionController = effectObject.GetComponentInChildren<WaveDistortionController>();
                    waveDistortionController.Deactivate();
                    break;
            }
        }

        private IEnumerator StopParticleSystem(GameObject effectObject)
        {
            //자식들의 모든 파티클 시스템을 찾아서 생성 중단 및 마지막 파티클 까지 대기
            ParticleSystem[] particleSystems = effectObject.GetComponentsInChildren<ParticleSystem>();

            if (particleSystems.Length == 0)
            {
                effectObject.SetActive(false);
                yield break;
            }

            foreach (var particle in particleSystems)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            float maxLifetime = 0f;
            foreach (var particle in particleSystems)
            {
                if (particle.main.startLifetime.constantMax > maxLifetime)
                {
                    maxLifetime = particle.main.startLifetime.constantMax;
                }
            }
            
            yield return new WaitForSeconds(maxLifetime);
            
            effectObject.SetActive(false);
        }

        private IEnumerator StopVFXGraph(GameObject effectObject)
        {
            //자식들의 모든 VFX 시스템을 찾아서 생성 중단 및 마지막 파티클 까지 대기
            VisualEffect[] vfxs = effectObject.GetComponentsInChildren<VisualEffect>();
            
            if (vfxs.Length == 0)
            {
                effectObject.SetActive(false);
                yield break;
            }
            
            // VFX 생성 중단
            foreach (var vfx in vfxs)
            {
                vfx.Stop();
            }
            
            // VFX가 완전히 종료될 때까지 대기
            bool anyAlive = true;
            float timeout = 5f; // 최대 5초 대기
            float elapsed = 0f;
            
            while (anyAlive && elapsed < timeout)
            {
                anyAlive = false;
                foreach (var vfx in vfxs)
                {
                    if (vfx.aliveParticleCount > 0)
                    {
                        anyAlive = true;
                        break;
                    }
                }
                
                if (anyAlive)
                {
                    elapsed += 0.1f;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            
            effectObject.SetActive(false);
        }
        #endregion
    }
}
