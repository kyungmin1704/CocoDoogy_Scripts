using CocoDoogy.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CocoDoogy.MiniGame.UmbrellaGame
{
    public class Umbrella : CanMoveImage
    {

        private UmbrellaSwipeMiniGame parent;
        private Sprite drySprite;
        bool isDry = false;
        public int needSwipeCount = 5;

        #region 우산 흔들기 판정 조건에 필요한 필드 
        // 거리 판정용 시간 (0.1초)
        [SerializeField] private float distanceCheckInterval = 0.1f;
        private float distanceCheckTimer = 0f;
        // 최소 실행 간격 (1초)
        [SerializeField] private float shakeCooldown = 0.3f;
        private float lastShakeTime = 0f;
        // 거리 누적
        private float accumulatedDistance = 0f;
        // 거리 기준: 이미지 크기 2배
        private float thresholdDistance;
        private Vector2 lastPosition;
        #endregion

        #region 핸드폰 흔들기를 감지하기위해 필요한 필드
         [SerializeField] private float shakeThreshold = 1.5f; // 감지 임계값
         [SerializeField] private float minIntervalShake = 0.3f;
         private float timeSinceLastShake = 0f;
        #endregion

        private void Start()
        {
            thresholdDistance = ((RectTransform)transform).rect.width*1.5f;
            lastPosition = transform.position;
            
            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
            }
            MiniGameParticleManager.Instance.ParticleWatering(transform);
        }
        
        public void Init(UmbrellaSwipeMiniGame umbrellaSwipeMiniGame)
        {
            parent = umbrellaSwipeMiniGame;
        }

        private void Update()
        {
            DetectShake();
        }

        /// <summary>
        /// UmbrellaSwipeMiniGame으로부터 젖은 우산이미지와 마른 우산이미지를 받음
        /// </summary>
        /// <param name="wetsprites"></param>
        /// <param name="drysprite"></param>
        public void SetSprites(Sprite wetsprites, Sprite drysprite)
        {
            image.sprite = wetsprites;
            drySprite = drysprite;
        }

        /// <summary>
        /// 마른이미지와 상태로 교체하면서 클리어 조건검사
        /// </summary>
        /// <param name="drysprite"></param>
        private void SetDry(Sprite drysprite)
        {
            isDry = true;
            image.sprite = drysprite;
            image.raycastTarget = false;
            parent.clearcount.Remove(this);
        }
        
        


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Handheld.Vibrate();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            SfxManager.ToggleLoopSound(SfxType.Loop_ShakeUmbrella);
        }

        /// <summary>
        /// 거리와 시간을 기반으로 우산의 흔들림을 감지하도록 설계
        /// 거리는 우산의 자체크기에 비례하도록하여(Start의 thresholdDistance = ((RectTransform)transform).rect.width*2;) 어떤 모바일상황에서도 대응될수 있도록 함 
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            Vector2 currentPos = transform.position;
            // 프레임 이동 거리 누적
            float frameDist = Vector2.Distance(currentPos, lastPosition);
            accumulatedDistance += frameDist;
            lastPosition = currentPos;
            // 0.1초 경과 체크
            distanceCheckTimer += Time.deltaTime;
            if (distanceCheckTimer >= distanceCheckInterval)
            {
                float now = Time.time;

                // 0.1초 누적거리 조건 충족 + 1초쿨다운 충족
                if (accumulatedDistance >= thresholdDistance &&
                    now - lastShakeTime >= shakeCooldown)
                {
                    if (needSwipeCount > 0)
                    {
                        needSwipeCount--;
                        parent.CheckClear();
                        //SfxManager.PlaySfx(SfxType.Minigame_ShakeUmbrella);
                    }
                    else if (!isDry)
                    {
                        SetDry(drySprite);
                        parent.CheckClear();
                        SfxManager.PlaySfx(SfxType.UI_Success);
                    }
                    // 쿨다운 시간 갱신
                    lastShakeTime = now;
                    Handheld.Vibrate();
                }
                // 누적 거리 초기화
                accumulatedDistance = 0f;
                // 타이머 초기화
                distanceCheckTimer = 0f;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            SfxManager.ToggleLoopSound(SfxType.Loop_ShakeUmbrella);
        }
        
        #region 모바일 환경
        /// <summary>
        /// 모바일기기 흔들기 감지(테스트 완)
        /// </summary>
        private void DetectShake()
        {
            if (isDry) return;

            timeSinceLastShake += Time.deltaTime;

            if (Accelerometer.current == null) return;

            Vector3 accel = Accelerometer.current.acceleration.ReadValue();
            if (timeSinceLastShake < minIntervalShake) return;

            if (accel.magnitude >= shakeThreshold)
            {
                timeSinceLastShake = 0f;
                Handheld.Vibrate();
                //SfxManager.PlaySfx(SfxType.Minigame_ShakeUmbrella);
                needSwipeCount--;
                if (needSwipeCount <= 0 && !isDry)
                {
                    SetDry(drySprite);
                    parent.CheckClear();
                    SfxManager.PlaySfx(SfxType.UI_SuccessMission);
                }
            }

        }
        #endregion
    }
}