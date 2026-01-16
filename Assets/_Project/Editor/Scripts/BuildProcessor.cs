using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoDoogy.Editor
{
    public class BuildProcessor
    {
        // Android Keystore 설정
        private const string KeystorePath = @"C:.\Assets\NotShared\user.keystore";
        private const string keystorePass = "qwer1234!@#$";
        private const string KeyaliasName = "CocoDoogy";
        private const string KeyaliasPass = "qwer1234!@#$";

        // Android versionCode (구글 플레이 업데이트용 내부 빌드 번호, 반드시 증가해야 함)
        private const string ArgName_BuildNum = "1";

        // 최종 빌드 파일이 생성될 전체 경로 (예: /workspace/build/app.apk 또는 .aab)
        private const string ArgName_OutputPath = @"./Builds/CocoDoogy.apk";

        // 빌드 타입 (apk 또는 aab) - aab면 AppBundle 빌드
        private const string ArgName_BuildType = "apk";

        // PlayerSettings.bundleVersion (사용자에게 보이는 앱 버전, 예: 1.0.3)
        private const string ArgName_BuildVersion = "v0.2.26";

        // Development Build 여부 (true면 Development Build로 컴파일)
        private const string ArgName_EnableDev = "fasle";

        // Deep Profiling 여부 (true면 deep profiling 빌드)
        private const string ArgName_EnableDeepProfiling = "false";

        // 출력될 파일명 (예: MyGame.apk) - outputPath와 합쳐져 최종 파일 이름이 됨
        private const string ArgName_OutputFileName = "CocoDoogy";

        private static string GetCommandLineArgument(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == $"-{name}" && i + 1 < args.Length)
                    return args[i + 1];
            }

            return null;
        }

        [MenuItem("Build/Build Android")]
        public static void BuildAndroid()
        {
            // BuildVersion 자동 증가
            string currentVersion = GetCommandLineArgument(ArgName_BuildVersion) ?? PlayerSettings.bundleVersion;
            string[] parts = currentVersion.Split('.');
            int major = int.Parse(parts[0]);
            int minor = int.Parse(parts[1]);
            int patch = int.Parse(parts[2]);

            patch++; // 이전 patch 값 +1 ex) v0.2.25 -> v0.2.26
            string newVersion = $"{major}.{minor}.{patch}";
            PlayerSettings.bundleVersion = newVersion;
            Debug.Log($"자동 증가된 BuildVersion: {newVersion}");

            AssetDatabase.SaveAssets(); // 변경 사항 저장 (다음 빌드에 반영하기 위해서)

            // BuildNum 자동 증가
            int buildNum = PlayerSettings.Android.bundleVersionCode + 1;
            PlayerSettings.Android.bundleVersionCode = buildNum;
            Debug.Log($"자동 증가된 BuildNum(versionCode): {buildNum}");

            // OutputPath 및 옵션 처리
            string outputPath = GetCommandLineArgument(ArgName_OutputPath) ?? "Builds/Android/";
            string extension = GetCommandLineArgument(ArgName_BuildType) ?? "apk";
            bool enableAab = extension == "aab";
            bool enableDev = GetCommandLineArgument(ArgName_EnableDev) == "true";
            bool enableDeepProfiling = GetCommandLineArgument(ArgName_EnableDeepProfiling) == "true";
            string outputFileName = GetCommandLineArgument($"{ArgName_OutputFileName}{newVersion}.{extension}")
                                    ?? $"CocoDoogy{newVersion}_{buildNum}.{extension}";

            if (Directory.Exists(outputPath))
            {
                outputPath = Path.Combine(outputPath, outputFileName);
            }
            else if (!outputPath.EndsWith(".apk") && !outputPath.EndsWith(".aab"))
            {
                outputPath = Path.Combine(outputPath, outputFileName);
            }

            Debug.Log($"최종 빌드 경로: {outputPath}");

            // BuildPlayerOptions
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = FindEnabledEditorScenes(),
                locationPathName = outputPath,
                target = BuildTarget.Android
            };

            EditorUserBuildSettings.buildAppBundle = enableAab;
            EditorUserBuildSettings.development = enableDev;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = enableDeepProfiling;


            // Keystore 설정
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = KeyaliasName;
            PlayerSettings.Android.keyaliasPass = KeyaliasPass;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log($"빌드 완료! 결과: {report.summary.result}, Path: {outputPath}");
        }
        private static string[] FindEnabledEditorScenes()
        {
            var editorScenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                editorScenes.Add(scene.path);
            }
            return editorScenes.ToArray();
        }
    }
}