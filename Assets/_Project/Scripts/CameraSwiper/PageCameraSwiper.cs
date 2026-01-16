using CocoDoogy.Audio;
using CocoDoogy.Utility;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;
using CocoDoogy.Core;

namespace CocoDoogy.CameraSwiper
{
    /*
    PageCameraSwiper (이벤트 기반 버전)
    ---------------------------------------
    - 스와이프 도중에도 부드럽게 방향 전환 가능
    - currentIndex 즉시 갱신 (반대 방향 입력 시 중앙으로 정확히 복귀)
    - Ease.OutCubic 사용 (중간 연결이 자연스럽고 튕김 제거)
    - 빠른 연속 스와이프에도 끊김 없이 자연스럽게 이어짐
    - 페이지 전환 시 OnPageChanged(int newIndex) 이벤트 호출
    - LightingManager 등 외부 시스템이 해당 이벤트를 구독하여 자동 반응 가능
    */

    public class PageCameraSwiper : MonoBehaviour
    {
        /// <summary>
        /// 페이지 전환 이벤트 (index 인자 포함)
        /// 외부 시스템(Lighting, BGM, UI 등)이 구독하여 페이지 변경에 반응
        /// </summary>
        public static event Action<Theme> OnStartPageChanged; // 페이지 전환 중

        public static event Action<Theme> OnEndPageChanged; // 페이지 전환 완료 시 호출

        public static Theme CurrentTheme { get; private set; } = Theme.None;

        
        [Header("Page Settings")]
        [SerializeField] private Transform[] cameraPoints;

        [SerializeField] private GameObject[] pages;

        [Header("Settings")]
        [Tooltip("드래그 감도 (값이 높을수록 민감)")]
        [SerializeField] private float dragSensitivity = 0.25f;

        [Tooltip("페이지 전환 임계값 (0~1, 화면 폭 대비 비율)")]
        [Range(0f, 1f)] public float snapThreshold = 0.25f;

        [Tooltip("전환 속도 (초 단위)")]
        [SerializeField] private float snapDuration = 0.5f;

        //대충 끌어다 쓰기 위한 스와이프 가능 여부
        public static bool IsSwipeable = true;

        private Camera mainCamera;
        private int currentIndex = 0; // 숲 = 0 , 물 , 눈 , 사막 = 4 (테마)

        private Vector2 startPos;
        private Vector2 lastPos;
        private bool isDragging = false;

        private float lerpSpeed = 12f; // 드래그 중 실시간 보간 속도


        void Start()
        {
            OnEndPageChanged += OnThemeChanged;
            if(PlayerPrefs.GetInt(Constants.Prefs.LOBBY_THEME) == 0)
            {
                PlayerPrefs.SetInt(Constants.Prefs.LOBBY_THEME, (int)Theme.Forest);
            }

            Theme theme = (Theme)PlayerPrefs.GetInt(Constants.Prefs.LOBBY_THEME);
            MoveToPageInstant(currentIndex = theme.ToIndex());
        }

        void OnDestroy()
        {
            OnEndPageChanged -= OnThemeChanged;
            DOTween.Kill(mainCamera);
        }

        void Update()
        {
            Swipe();
        }


        private Theme GetThemeByIndex(int index) => index < 0 ? Theme.None : (Theme)(1 << index);

        private void Swipe()
        {
            if (TouchSystem.TouchCount > 0 && IsSwipeable)
            {
                lastPos = TouchSystem.TouchAverage;

                if (!isDragging)
                {
                    isDragging = true;
                    startPos = lastPos;

                    OnStartPageChanged?.Invoke(GetThemeByIndex(currentIndex));
                }

                float deltaX = (lastPos.x - startPos.x) / Screen.width;
                float dragPercent = Mathf.Clamp(deltaX * dragSensitivity, -1f, 1f);

                int targetIndex = currentIndex;
                if (dragPercent > 0 && currentIndex > 0)
                    targetIndex = currentIndex - 1;
                else if (dragPercent < 0 && currentIndex < cameraPoints.Length - 1)
                    targetIndex = currentIndex + 1;

                float weight = Mathf.Abs(dragPercent);
                Transform currentPoint = cameraPoints[currentIndex];
                Transform targetPoint = cameraPoints[targetIndex];

                Vector3 blendedPos = Vector3.Lerp(currentPoint.position, targetPoint.position, weight);
                Quaternion blendedRot = Quaternion.Slerp(currentPoint.rotation, targetPoint.rotation, weight);

                mainCamera.transform.position =
                    Vector3.Lerp(mainCamera.transform.position, blendedPos, Time.deltaTime * lerpSpeed);
                mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, blendedRot,
                    Time.deltaTime * lerpSpeed);
            }
            else if (isDragging)
            {
                isDragging = false;
                float normalizedDrag = (lastPos.x - startPos.x) / Screen.width;
                EvaluateSwipe(normalizedDrag);
            }
        }

        // 드래그 종료 시 비율 계산 및 방향 판단
        private void EvaluateSwipe(float normalizedDrag)
        {
            int newIndex = currentIndex;

            if (Mathf.Abs(normalizedDrag) > snapThreshold)
                newIndex = normalizedDrag > 0 ? currentIndex - 1 : currentIndex + 1;

            newIndex = Mathf.Clamp(newIndex, 0, cameraPoints.Length - 1);
            MoveToPageSmooth(newIndex);
            SfxManager.PlaySfx(SfxType.UI_ButtonUp2);

            // 다음 스와이프를 위해 기준점 갱신
            startPos = lastPos;
        }

        // DOTween 기반 스무스 카메라 이동
        private void MoveToPageSmooth(int index)
        {
            // 기존 트윈 중단 후 새 트윈 시작
            DOTween.Kill(mainCamera);

            Transform camTr = mainCamera.transform;
            Transform targetPoint = cameraPoints[index];

            // 전환 시 현재 + 다음 페이지만 활성화
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] == null) continue;
                bool shouldBeActive = (i == currentIndex) || (i == index);
                pages[i].SetActive(shouldBeActive);
            }

            // currentIndex 즉시 갱신 (중간에 반대 스와이프 시 정확한 기준 유지)
            currentIndex = index;

            // 페이지 변경 이벤트 즉시 호출 (Lighting, BGM 등이 카메라 애니메이션과 동시에 전환)
            OnEndPageChanged?.Invoke(GetThemeByIndex(index));

            camTr.DOMove(targetPoint.position, snapDuration)
                .SetEase(Ease.OutCubic)
                .SetId(mainCamera);

            camTr.DORotateQuaternion(targetPoint.rotation, snapDuration)
                .SetEase(Ease.OutCubic)
                .SetId(mainCamera)
                .OnComplete(() =>
                {
                    SetActivePage(index);
                });
        }

        private void MoveToPageInstant(int index)
        {
            mainCamera = Camera.main;
            IsSwipeable = true;

            mainCamera.transform.position = cameraPoints[index].position;
            mainCamera.transform.rotation = cameraPoints[index].rotation;

            OnEndPageChanged?.Invoke(CurrentTheme = GetThemeByIndex(index));
            SetActivePage(index);
        }

        private void SetActivePage(int activeIndex)
        {
            for (int i = 0; i < pages.Length; i++)
                pages[i].SetActive(i == activeIndex);
        }
        

        private void OnThemeChanged(Theme theme)
        {
            PlayerPrefs.SetInt(Constants.Prefs.LOBBY_THEME, (int)theme);
        }
    }
}