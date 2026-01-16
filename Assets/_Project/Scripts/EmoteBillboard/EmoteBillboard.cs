using CocoDoogy.Audio;
using UnityEngine;
using DG.Tweening;
using System;

namespace CocoDoogy.EmoteBillboard
{
    // 어느 방향에서든 이모지가 제대로 보이도록 하는 스크립트
    public class EmoteBillboard : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("이모트를 띄울 타겟 오브젝트")]
        [SerializeField] private Transform target; // 타겟 오브젝트
        [Tooltip("타겟과의 거리 오프셋")]
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

        [Header("Emotion Sprites")]
        [Tooltip("감정별 스프라이트 (Emotion enum 순서대로)")]
        [SerializeField] private Sprite[] emotionSprites = new Sprite[7];

        private float displayDuration = 1.6f; // 표시 유지 시간
        private float fadeOutDuration = 0.4f; // 페이드아웃 시간
        private float fadeInDuration = 0.15f; // 페이드인 시간
        private float finalAlpha = 0.1f; // 최종 알파값

        private float popInScaleY = 1.5f; // 세로 팝업 스케일 (더 길게)
        private float popInScaleX = 1.3f; // 가로 팝업 스케일
        private float popInDuration = 0.25f; // 팝업 애니메이션 시간
        private float shrinkDuration = 0.3f; // 축소 애니메이션 시간
        private float shrinkScale = 0.8f; // 축소 시작 스케일

        private float duration = 5f; // 기존 ShowEmote() 메서드용 유지 시간
        private float fadeTime = 0.15f; // 기존 ShowEmote() 메서드용 페이드 시간

        private Camera mainCamera; // 메인 카메라 참조
        private SpriteRenderer spriteRenderer; // 이모트의 스프라이트 렌더러
        private Vector3 startPos; // 이모트의 초기 위치
        private Vector3 originalScale; // 이모트의 원래 크기
        private Sequence currentSequence; // 이모트가 움직이는 애니메이션을 위한 시퀀스
        private Action onCompleteCallback; // 감정 표시 완료 콜백

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;

            // 초기 상태: 비활성화
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate() // 여기서 계속 위치를 초기화 해서 카메라가 움직여도 무조건 타겟 위에 이모지가 보이도록 
        {
            if (!target || !mainCamera) return;

            // 카메라 기준 방향 벡터
            Vector3 camForward = (target.position - mainCamera.transform.position).normalized;

            // 카메라의 위쪽 벡터 (시야 기준 위)
            Vector3 camUp = mainCamera.transform.up;

            // 월드 기준이 아닌, 카메라 기준 으로 offset 적용
            Vector3 viewOffset = camUp * offset.y;        // 머리 위로 이동
            Vector3 viewForwardOffset = camForward * offset.z; // 카메라 쪽으로 약간 당기기
            Vector3 viewSideOffset = mainCamera.transform.right * offset.x; // 좌우 미세조정 가능

            // 이모트의 위치를 카메라 기준 위치로 설정 (아무 위치에서든 타겟 위에 이모지가 보이도록)
            transform.position = target.position + viewOffset + viewForwardOffset + viewSideOffset;

            // 카메라를 바라보게 설정 (어느 방향에서든 이모지의 정면(Z축)이 카메라를 바라보도록)
            transform.LookAt(
                transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up
            );
        }

        /// <summary>
        /// 감정 표시
        /// </summary>
        /// <param name="emotion">표시할 감정</param>
        /// <param name="onComplete">표시 완료 콜백</param>
        public void ShowEmotion(Emotion emotion, Action onComplete = null)
        {
            if (emotion == Emotion.None) return;
            if (!spriteRenderer) return; // null 체크

            onCompleteCallback = onComplete;

            // 이전 Tween 정리
            if (currentSequence != null && currentSequence.IsActive())
                currentSequence.Kill();

            // 감정에 맞는 스프라이트 설정
            int emotionIndex = (int)emotion;
            if (emotionIndex >= 0 && emotionIndex < emotionSprites.Length && emotionSprites[emotionIndex] != null)
            {
                spriteRenderer.sprite = emotionSprites[emotionIndex];
            }

            // 초기화
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            transform.localScale = new Vector3(0, 0, 1); // 0에서 시작 (Z축은 유지)
            startPos = target ? target.position + offset : transform.position;
            transform.position = startPos;
            
            //Sfx 실행
            PlayEmoteSfx(emotion);
            
            // 애니메이션 시퀀스
            currentSequence = DOTween.Sequence();

            // 1단계: 세로로 늘어남 (Y축만 확장)
            currentSequence.Append(transform.DOScaleY(originalScale.y * popInScaleY, popInDuration * 0.5f).SetEase(Ease.OutQuad));
            currentSequence.Join(spriteRenderer.DOFade(1f, popInDuration * 0.5f).SetEase(Ease.Linear));

            // 2단계: 가로로 늘어남 (X축 확장)
            currentSequence.Append(transform.DOScaleX(originalScale.x * popInScaleX, popInDuration * 0.5f).SetEase(Ease.OutQuad));

            // 3단계: 정상 크기로 복귀 (가로세로 동시)
            currentSequence.Append(transform.DOScale(originalScale, 0.1f).SetEase(Ease.InOutQuad));

            // 4단계: 유지
            currentSequence.AppendInterval(displayDuration);

            // 5단계: 가로로 줄어들면서 페이드 아웃
            currentSequence.Append(transform.DOScaleX(0, fadeOutDuration).SetEase(Ease.InQuad));
            currentSequence.Join(spriteRenderer.DOFade(finalAlpha, fadeOutDuration).SetEase(Ease.Linear));

            // 완료 처리
            currentSequence.OnComplete(() =>
            {
                Color finalColor = spriteRenderer.color;
                finalColor.a = 0;
                spriteRenderer.color = finalColor;
                transform.localScale = originalScale;

                onCompleteCallback?.Invoke();
                onCompleteCallback = null;
            });
        }

        /// <summary>
        /// 기존 메서드 (하위 호환용)
        /// </summary>
        public void ShowEmote()
        {
            // 이전 Tween 정리
            if (currentSequence != null && currentSequence.IsActive())
                currentSequence.Kill();

            // 초기화
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            transform.localScale = originalScale;
            startPos = target ? target.position + offset : transform.position;
            transform.position = startPos;

            // 새 Sequence 생성
            currentSequence = DOTween.Sequence();

            // 페이드 인
            currentSequence.Append(spriteRenderer.DOFade(1f, fadeTime).SetEase(Ease.Linear));

            // 유지 (duration초 동안 완전히 표시)
            currentSequence.AppendInterval(duration);

            // 페이드 아웃
            currentSequence.Append(spriteRenderer.DOFade(0f, fadeTime).SetEase(Ease.Linear));

            // 종료 후 초기화
            currentSequence.OnComplete(() =>
            {
                Color finalColor = spriteRenderer.color;
                finalColor.a = 0;
                spriteRenderer.color = finalColor;
                transform.localScale = originalScale;
            });
        }

        private void OnDisable()
        {
            if (currentSequence != null)
                currentSequence.Kill();
        }
        
        #region SFX
        private static readonly System.Collections.Generic.Dictionary<Emotion, SfxType> EmotionSfxMap = 
            new System.Collections.Generic.Dictionary<Emotion, SfxType>
            {
                { Emotion.Joy, SfxType.Emote_Positive },
                { Emotion.Satisfaction, SfxType.Emote_Positive },
                { Emotion.Boredom, SfxType.Emote_Neutral },
                { Emotion.Sad, SfxType.Emote_Negative },
                { Emotion.Thrill, SfxType.Emote_Negative },
                { Emotion.Anxiety, SfxType.Emote_Negative },
            };
        
        private void PlayEmoteSfx(Emotion emotion)
        {
            if (EmotionSfxMap.TryGetValue(emotion, out SfxType sfxType))
            {
                SfxManager.PlaySfx(sfxType);
            }
        }
        #endregion
    }
}