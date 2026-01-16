using UnityEngine;
using UnityEngine.Rendering;

namespace CocoDoogy.CameraSwiper.Lighting
{
    /// <summary>
    /// Lighting / Environment / Other 세팅을 모두 포함하는 ScriptableObject
    /// 각 페이지마다 다른 분위기를 위해 사용
    /// </summary>
    [CreateAssetMenu(fileName = "LightingPreset", menuName = "Game/Lighting Preset")]
    public class LightingPreset : ScriptableObject
    {
#if UNITY_EDITOR
        [Header("=== Lighting Settings (에디터용) ===")]
        [Tooltip("Baked Lighting 설정 (에디터 전용)")]
        public LightingSettings bakedLighting; // Editor only
#endif

        [Header("=== Environment Settings ===")]
        [Tooltip("스카이박스 머티리얼")]
        public Material skybox;

        [Tooltip("앰비언트 모드")]
        public AmbientMode ambientMode = AmbientMode.Skybox;

        [Tooltip("앰비언트 스카이 색상")]
        public Color ambientSkyColor = Color.white;

        [Tooltip("앰비언트 적도 색상")]
        public Color ambientEquatorColor = Color.gray;

        [Tooltip("앰비언트 지면 색상")]
        public Color ambientGroundColor = Color.black;

        [Header("Fog")]
        [Tooltip("안개 사용 여부")]
        public bool useFog = false;

        [Tooltip("안개 모드")]
        public FogMode fogMode = FogMode.ExponentialSquared;

        [Tooltip("안개 색상")]
        public Color fogColor = Color.gray;

        [Tooltip("안개 밀도")]
        public float fogDensity = 0.01f;

        [Tooltip("안개 시작 거리")]
        public float fogStart = 0f;

        [Tooltip("안개 종료 거리")]
        public float fogEnd = 100f;

        [Header("Sun Light")]
        [Tooltip("태양 라이트 (메인 라이트)")]
        public Light sun;

        [Header("=== Other Settings ===")]
        [Tooltip("감산 그림자 색상")]
        public Color subtractiveShadowColor = Color.black;

        [Tooltip("헤일로 강도")]
        public float haloStrength = 0.5f;

        [Tooltip("플레어 강도")]
        public float flareStrength = 1.0f;

        [Tooltip("플레어 페이드 속도")]
        public float flareFadeSpeed = 3.0f;

        /// <summary>
        /// 현재 씬의 RenderSettings 값을 이 LightingPreset에 복사
        /// </summary>
        public void CopyFromRenderSettings()
        {
            // === Environment Settings ===
            skybox = RenderSettings.skybox;
            ambientMode = RenderSettings.ambientMode;
            ambientSkyColor = RenderSettings.ambientSkyColor;
            ambientEquatorColor = RenderSettings.ambientEquatorColor;
            ambientGroundColor = RenderSettings.ambientGroundColor;

            // === Fog ===
            useFog = RenderSettings.fog;
            fogMode = RenderSettings.fogMode;
            fogColor = RenderSettings.fogColor;
            fogDensity = RenderSettings.fogDensity;
            fogStart = RenderSettings.fogStartDistance;
            fogEnd = RenderSettings.fogEndDistance;

            // === Sun Light ===
            sun = RenderSettings.sun;

            // === Other Settings ===
            subtractiveShadowColor = RenderSettings.subtractiveShadowColor;
            haloStrength = RenderSettings.haloStrength;
            flareStrength = RenderSettings.flareStrength;
            flareFadeSpeed = RenderSettings.flareFadeSpeed;
        }
    }
}

