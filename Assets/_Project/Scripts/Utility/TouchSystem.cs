using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace CocoDoogy.Utility
{
    public static class TouchSystem
    {
        public enum InputType
        {
            None,
            Mouse,
            Touch
        }


        /// <summary>
        /// 현재 Input 모드
        /// </summary>
        public static InputType CurrentInputType { get; private set; } = InputType.None;


        /// <summary>
        /// 현재 터치된 위치의 중간위치
        /// </summary>
        public static Vector2 TouchAverage
        {
            get
            {
                switch(CurrentInputType)
                {
                    case InputType.Mouse:
                        return Mouse.current.position.ReadValue();
                    case InputType.Touch:
                        int count = 0;
                        Vector2 average = Vector2.zero;
                        foreach (var touch in Touchscreen.current.touches)
                        {
                            if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                            {
                                average += touch.position.value;
                                count++;
                            }
                        }
                        return average / count;
                    default:
                        return Vector2.zero;
                }
            }
        }
        /// <summary>
        /// 터치 중심 위치에서 벌어진 터치 포인트들의 평균값
        /// </summary>
        public static float DistanceAverage
        {
            get
            {
                if(CurrentInputType == InputType.Touch)
                {
                    Vector2 center = TouchAverage;
                    float average = 0f;
                    int count = 0;
                    foreach (var touch in Touchscreen.current.touches)
                    {
                        if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                        {
                            average += Vector2.Distance(center, touch.position.value);
                            count++;
                        }
                    }
                    return average / count;
                }
                
                return 0f;
            }
        }

        /// <summary>
        /// 현재 터치 중인 위치값 개수
        /// </summary>
        public static int TouchCount
        {
            get
            {
                switch(CurrentInputType)
                {
                    case InputType.Mouse:
                        return Mathf.RoundToInt(Mouse.current.leftButton.value);
                    case InputType.Touch:
                        int count = 0;
                        foreach (var touch in Touchscreen.current.touches)
                        {
                            if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                            {
                                count++;
                            }
                        }
                        return count;
                    default:
                        return 0;
                }
            }
        }
        
        /// <summary>
        /// 현재 터치 or 클릭이 UI 위에서 이뤄졌는지
        /// </summary>
        public static bool IsPointerOverUI
        {
            get
            {
                switch(CurrentInputType)
                {
                    case InputType.Mouse:
                        return EventSystem.current.IsPointerOverGameObject();
                    case InputType.Touch:
                        foreach (var touch in Touchscreen.current.touches)
                        {
                            if (touch.isInProgress &&
                                EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue()))
                                return true;
                        }
                        return false;
                    default:
                        return false;
                }
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitRuntime()
        {
            InputSystem.onEvent += OnInputEvent;
        }

        private static void OnInputEvent(InputEventPtr ptr, InputDevice device)
        {
            if(!ptr.IsA<StateEvent>() && !ptr.IsA<DeltaStateEvent>()) return;


            if(device is Mouse)
            {
                CurrentInputType = InputType.Mouse;
            }
            else if(device is Touchscreen)
            {
                CurrentInputType = InputType.Touch;
            }
        }
    }
}