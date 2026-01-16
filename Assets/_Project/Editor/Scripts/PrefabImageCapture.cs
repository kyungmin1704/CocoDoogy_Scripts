using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace CocoDoogy.Editor
{
    public class PrefabImageCapture
    {
        [MenuItem("Tools/Prefab Thumbnail/Generate All Prefabs in Folder")]
        public static void GenerateAllPrefabs()
        {
            // 🔹 읽어올 폴더 경로 (Assets 하위)
            string folderPath = "Assets/NotShared/_Building Prefabs";
            string saveFolder = "Assets/NotShared/_Building Prefabs/Prefab Images";

            // 🔹 폴더 내 모든 Prefab 경로 읽기
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            if (prefabGUIDs.Length == 0)
            {
                Debug.LogWarning("⚠️ 해당 폴더에 프리팹이 없습니다: " + folderPath);
                return;
            }

            Directory.CreateDirectory(saveFolder);

            foreach (string guid in prefabGUIDs)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab == null) continue;

                Texture2D preview = GetPreviewBlocking(prefab);
                if (preview == null)
                {
                    Debug.LogWarning($"⚠️ 썸네일을 가져올 수 없음: {prefab.name}");
                    continue;
                }

                // Texture → Sprite 생성
                Rect rect = new Rect(0, 0, preview.width, preview.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(preview, rect, pivot);
                sprite.name = prefab.name + "_Thumbnail";

                // PNG로 저장
                string savePath = Path.Combine(saveFolder, prefab.name + "_Thumbnail.png");
                File.WriteAllBytes(savePath, preview.EncodeToPNG());

                Debug.Log($"✅ 저장 완료: {prefab.name}");
            }

            AssetDatabase.Refresh();
            Debug.Log($"🎉 모든 프리팹 썸네일 저장 완료! 총 {prefabGUIDs.Length}개");
        }

        /// <summary>
        /// 프리팹 썸네일 생성 완료될 때까지 대기
        /// </summary>
        private static Texture2D GetPreviewBlocking(Object obj)
        {
            Texture2D preview = null;
            int tries = 0;
            while (preview == null && tries < 50) // 최대 5초 대기
            {
                preview = AssetPreview.GetAssetPreview(obj);
                if (preview != null) break;
                Thread.Sleep(100);
                tries++;
            }
            return preview;
        }
    }
}