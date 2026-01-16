using UnityEngine;
using Lean.Pool;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CocoDoogy.EffectPooling
{
    public class TouchEffectEmitter : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("터치 이펙트 프리팹")]
        public GameObject touchEffectPrefab;

        [Header("설정")]
        [Tooltip("터치 중복 처리")]
        public bool onlyOneFinger = false;

        [Tooltip("UI canvas")]
        public RectTransform canvasRect;

        private void Start()
        {
            if (canvasRect == null)
                canvasRect = GetComponent<RectTransform>();
        }


        private void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            TestWithOldInputSystem();
#elif ENABLE_INPUT_SYSTEM
            TestWithNewInputSystem();
#endif
        }


#if ENABLE_INPUT_SYSTEM
        private void TestWithNewInputSystem()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.wasPressedThisFrame)
                    {
                        SpawnTouchEffect(touch.position.ReadValue());
                        if (onlyOneFinger) break;
                    }
                }
            }
#else
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                SpawnTouchEffect(Mouse.current.position.ReadValue());
            }
#endif
        }
#endif


#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
        private void TestWithOldInputSystem()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount > 0)
            {
                int count = onlyOneFinger ? 1 : Input.touchCount;

                for (int i = 0; i < count; i++)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                        SpawnTouchEffect(Input.GetTouch(i).position);
                }
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                SpawnTouchEffect(Input.mousePosition);
            }
#endif
        }
#endif


        private void SpawnTouchEffect(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPosition, null, out Vector2 anchoredPos
            );

            // Instantiate 대신 LeanPool.Spawn 사용
            GameObject effect = LeanPool.Spawn(touchEffectPrefab, canvasRect.transform);

            // RectTransform 위치 세팅
            effect.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        }
    }
}
