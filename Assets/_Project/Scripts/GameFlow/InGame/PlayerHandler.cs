using CocoDoogy.Animation;
using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Phase;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Tutorial;
using CocoDoogy.UI.Replay;
using CocoDoogy.UI.UIManager;
using CocoDoogy.Utility;
using DG.Tweening;
using System;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class PlayerHandler : Singleton<PlayerHandler>
    {
        public static event Action<Vector2Int, PlayerEventType> OnEvent = null;
        public static Action<Vector2Int, PlayerEventType> OnEventCallback => OnEvent;


        /// <summary>
        /// 플레이어가 인게임에 들어와서 행동을 했는지 여부
        /// </summary>
        public static bool IsBehaviour { get; set; } = false;
        public static bool Locked { get; set; } = false;

        /// <summary>
        /// 현재 플레이 하고있는 씬이 Replay면 true로, InGame이면 false로
        /// </summary>
        public static bool IsReplay { get; set; } = false;

        public static int SandCount { get; set; } = 0;

        public static Vector2Int GridPos
        {
            get => Instance?.gridPos ?? Vector2Int.zero;
            set
            {
                if (!IsValid) return;
                if (Instance.gridPos == value) return;

                Instance.gridPos = value;
            }
        }

        public static HexDirection LookDirection
        {
            get => Instance?.lookDirection ?? HexDirection.East;
            set
            {
                if (!IsValid) return;
                if (Instance.lookDirection == value) return;

                Instance.lookDirection = value;
                Instance.transform.DORotate(new Vector3(0, value.ToDegree(), 0), Constants.MOVE_DURATION);
            }
        }


        /// <summary>
        /// 현재 플레이어가 동작할 수 있을지 판단
        /// </summary>
        public static bool IsValid
        {
            get
            {
                if (!Instance) return false;

                return true;
            }
        }

        private static Vector2Int prevGridPos = Vector2Int.zero;

        private Vector2Int gridPos = Vector2Int.zero;
        private HexDirection lookDirection = HexDirection.East;
        private PlayerAnimHandler anim = null;

        private bool isMoving = false;

        private Camera mainCamera = null;
        private bool touched = false;
        private Vector2 touchStart = Vector2.zero;
        private Vector2 touchLast = Vector2.zero;
        private int touchCount = 0;

        private ClearCheckPhase replayPhase = new();

        protected override void Awake()
        {
            base.Awake();

            anim = GetComponentInChildren<PlayerAnimHandler>();
        }
        void Start()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (isMoving) return;
            if (Locked) return;
            if (TouchSystem.IsPointerOverUI) return;
            if (IsReplay) return;

            if (TouchSystem.TouchCount > 0)
            {
                touchLast = TouchSystem.TouchAverage;
                if (TouchSystem.TouchCount != touchCount)
                {
                    touched = true;
                    touchStart = touchLast;
                    touchCount = TouchSystem.TouchCount;
                    return;
                }

                float distance = Vector2.Distance(touchLast, touchStart);
                if (distance > 20) // TODO: 값은 나중에 바뀔 수 있음
                {
                    touched = false;
                }
            }
            else
            {
                touchCount = 0;

                if (!touched) return;
                touched = false;

                Ray ray = mainCamera.ScreenPointToRay(touchLast);
                HexTile selectedTile = GetRayTile(ray);
                if (!selectedTile) return;
                if (!TutorialLocker.CanPos(selectedTile.GridPos)) return;

                HexDirection? direction = GridPos.GetRelativeDirection(selectedTile.GridPos);
                if (!direction.HasValue) return;

                HexTile playerTile = HexTile.GetTile(GridPos);
                if (!playerTile.CanMove(direction.Value))
                {
                    SfxManager.PlaySfx(SfxType.UI_FailButton2);
                    return;
                }

                CommandManager.Move(direction.Value);
            }
        }
        /// <summary>
        /// Ray에 부딪힌 HexTile을 반환
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        private HexTile GetRayTile(Ray ray)
        {
            HexTile result = null;
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Tile")))
            {
                result = hit.collider.GetComponentInParent<HexTile>(); ;
            }
            return result;
        }


        public static void Clear()
        {
            if (!IsValid) return;

            SandCount = 0;
        }


        /// <summary>
        /// 게임 시작 시 첫 위치 배치
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Deploy(Vector2Int gridPos)
        {
            if (!IsValid) return;

            // 추후 Move 및 Slide에서 사용할지 고민 좀 해봐야할 듯 함
            Vector2Int? preGravityButton = null;
            if (HexTile.GetTile(GridPos)?.HasPiece(PieceType.GravityButton, out _) ?? false)
            {
                preGravityButton = GridPos;
            }

            Instance.transform.parent = null;
            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.transform.position = gridPos.ToWorldPos();
            if (preGravityButton.HasValue) // 실제 기존 발판 리셋하는 곳
            {
                GimmickExecutor.ExecuteFromTrigger(preGravityButton
                    .Value); // Deploy는 갑자기 위치가 바뀌는 문제라 발판이 해결 안 되는 사태를 대비
            }
            // TODO : 이펙트 추가
            VfxManager.CreateVfx(VfxType.None, Instance.transform.position, Instance.transform.rotation);
            OnBehaviourCompleted();
        }

        public static void Comeback(Vector2Int gridPos)
        {
            if (!IsValid) return;


        }

        /// <summary>
        /// 토네이도용 이동방식
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Tornado(Vector2Int gridPos)
        {
            if (!IsValid) return;

            Instance.isMoving = true;

            Instance.transform.parent = null;
            GridPos = gridPos;
            DOTween.Kill(Instance, true);

            Sequence sequence = DOTween.Sequence();
            sequence.SetId(Instance);
            sequence.Append(Instance.transform.DOMoveY(10, Constants.MOVE_DURATION));
            sequence.Append(Instance.transform.DOMove(GridPos.ToWorldPos() + Vector3.up * 10, Constants.MOVE_DURATION));
            sequence.Append(Instance.transform.DOMove(GridPos.ToWorldPos(), Constants.MOVE_DURATION));
            sequence.OnComplete(OnBehaviourCompleted);
            sequence.Play();

            // 추후 Move 및 Slide에서 사용할지 고민 좀 해봐야할 듯 함
            Vector2Int? preGravityButton = null;
            if (HexTile.GetTile(GridPos)?.HasPiece(PieceType.GravityButton, out _) ?? false)
            {
                preGravityButton = GridPos;
            }
            if (preGravityButton.HasValue) // 실제 기존 발판 리셋하는 곳
            {
                GimmickExecutor.ExecuteFromTrigger(preGravityButton
                    .Value); // Deploy는 갑자기 위치가 바뀌는 문제라 발판이 해결 안 되는 사태를 대비
            }
        }

        /// <summary>
        /// 보행으로 이동
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Move(Vector2Int gridPos)
        {
            print("Move호출");
            if (!IsValid) return;
            if (!IsBehaviour) IsBehaviour = true;

            if(!IsReplay)InGameUIManager.Instance.OnInteractButtonActive();

            Instance.isMoving = true;
            Instance.transform.parent = null;
            prevGridPos = GridPos;
            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Moving);
            Instance.PlayFootstepCoroutine();
            DOTween.Kill(Instance, true);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION)
                .SetId(Instance)
                .OnComplete(OnBehaviourCompleted);
        }

        /// <summary>
        /// 미끄러지듯 이동
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Slide(Vector2Int gridPos)
        {
            if (!IsValid) return;

            Instance.isMoving = true;

            Instance.transform.parent = null;

            float distance = Vector3.Distance(Instance.transform.position, gridPos.ToWorldPos());

            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Slide);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.SLIDE_PER_DURATION * distance).SetId(Instance)
                .OnComplete(OnBehaviourCompleted);
        }


        public static void OnBehaviourCompleted()
        {
            DOTween.Kill(Instance, false);
            Instance.anim.ChangeAnim(AnimType.Idle);
            Instance.isMoving = false;
            if (!IsReplay)
            {
                InGameManager.ProcessPhase();
                OnEvent?.Invoke(GridPos, PlayerEventType.Move);
            }
            else
            {
                Instance.replayPhase.OnPhase();
            }
        }


        //발소리 코루틴
        private void PlayFootstepCoroutine()
        {
            HexTile currentTile = HexTile.GetTile(prevGridPos);
            if (!currentTile) return;
            if (currentTile.CurrentData.stepSfx == SfxType.None) return;

            SfxManager.PlaySfx(currentTile.CurrentData.stepSfx);
        }
    }
}