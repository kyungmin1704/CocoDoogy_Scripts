#if UNITY_EDITOR
using CocoDoogy.CameraSwiper.Lighting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoDoogy.Editor
{
    /// <summary>
    /// 현재 씬의 Lighting 설정을 LightingPreset ScriptableObject로 저장하는 에디터 유틸리티
    /// Tools → Lighting → Save Current Scene LightingPreset 메뉴에서 사용 가능
    /// </summary>
    public static class LightingPresetSaver
    {
        [MenuItem("Tools/Lighting/Save Current Scene LightingPreset")]
        public static void SaveCurrentSceneLighting()
        {
            // 새 ScriptableObject 생성
            LightingPreset preset = ScriptableObject.CreateInstance<LightingPreset>();

            // 현재 씬의 RenderSettings 값들을 모두 복사
            preset.CopyFromRenderSettings();

#if UNITY_EDITOR
            // Lighting Settings (에디터 전용)
            // Lightmapping.lightingSettings는 현재 씬의 Lighting Settings를 반환
            var lightingSettings = Lightmapping.lightingSettings;
            if (lightingSettings != null)
            {
                preset.bakedLighting = lightingSettings;
            }
#endif

            // 저장 경로 선택
            string sceneName = SceneManager.GetActiveScene().name;
            string defaultFileName = $"LightingPreset_{sceneName}";

            string path = EditorUtility.SaveFilePanelInProject(
                "Save LightingPreset",
                defaultFileName,
                "asset",
                "현재 씬의 LightingPreset을 저장합니다."
            );

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("[LightingPresetSaver] 저장이 취소되었습니다.");
                return;
            }

            // Asset 생성 및 저장
            AssetDatabase.CreateAsset(preset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[LightingPresetSaver] LightingPreset 저장 완료: {path}");

            // 생성된 Asset을 선택 (Inspector에서 확인 가능)
            Selection.activeObject = preset;
            EditorGUIUtility.PingObject(preset);
        }
    }
}
#endif

