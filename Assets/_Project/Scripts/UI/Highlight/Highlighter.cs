using CocoDoogy.Core;
using CocoDoogy.Tutorial;
using UnityEngine;

namespace CocoDoogy.UI.Highlight
{
    public class Highlighter: Singleton<Highlighter>
    {
        private static bool IsValid => Instance;


        [SerializeField] private RectTransform highlightRect;


        private Camera mainCamera = null;


        protected override void Awake()
        {
            base.Awake();
            Invisible();
        }

        void Start()
        {
            mainCamera = Camera.main;
        }


        /// <summary>
        /// UI로 이동
        /// </summary>
        public static void FocusUI(Vector2 pos)
        {
            if(!IsValid) return;
            Instance.highlightRect.gameObject.SetActive(true);
            Instance.highlightRect.position = pos;
            TutorialUI.OffBackground();
        }
        /// <summary>
        /// GridPos로 이동
        /// </summary>
        public static void FocusTile(Vector2Int gridPos)
        {
            if(!IsValid) return;
            Instance.highlightRect.gameObject.SetActive(true);
            Instance.highlightRect.position = Instance.mainCamera.WorldToScreenPoint(gridPos.ToWorldPos());
            TutorialUI.OffBackground();
        }

        /// <summary>
        /// 하이라이트 끄기
        /// </summary>
        public static void Invisible()
        {
            if(!IsValid) return;
            Instance.highlightRect.gameObject.SetActive(false);
            TutorialUI.OnBackground();
        }
    }
}