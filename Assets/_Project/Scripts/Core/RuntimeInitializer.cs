using UnityEngine;

namespace CocoDoogy.Core
{
    public static class RuntimeInitializer
    {
        /// <summary>
        /// 게임 실행 시, DontDestroy해야 하는 모든 Manager 스크립트를 갖고 있는<br/>
        /// CoreManager 생성 메소드
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            GameObject.Instantiate(Resources.Load<GameObject>("CoreManager")).name = "CoreManager";
            Application.targetFrameRate = 60;
        }
    }
}