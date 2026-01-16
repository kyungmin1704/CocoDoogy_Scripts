using UnityEditor;

using System.Linq;

namespace CocoDoogy.Editor
{
    /// <summary>
    /// CI용 클래스
    /// </summary> 
    public static class BuildAutomator
    {
        private const string KeystorePath = "./Assets/NotShared/user.keystore"; // 네 경로로 수정
        private const string KeystorePass = "qwer1234!@#$";   // 네 keystore 비번a
        private const string AliasName = "CocoDoogy";      // 네 alias 이름
        private const string AliasPass = "qwer1234!@#$";   // alias 비번

        // PlayerSettings.bundleVersion (사용자에게 보이는 앱 버전, 예: 1.0.3)
        private const string ArgName_BuildVersion = "v0.2.26";

        // 출력될 파일명 (예: MyGame.apk) - outputPath와 합쳐져 최종 파일 이름이 됨
        private const string ArgName_OutputFileName = "CocoDoogy";


        [MenuItem("Build/Windows")]
        public static void BuildForWindows()
        {
            ApplyAndroidKeystoreSettings();
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {
            ApplyAndroidKeystoreSettings();
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

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

        private static string[] GetScenesFromBuildSettings()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
        }

        private static void ApplyAndroidKeystoreSettings()
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = AliasName;
            PlayerSettings.Android.keyaliasPass = AliasPass;
        }
    }
}