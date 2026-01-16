using CocoDoogy.Audio;
using CocoDoogy.Data;
using CocoDoogy.EffectPooling;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.LifeCycle;
using CocoDoogy.Tile.Piece;
using DG.Tweening;
using EPOOutline;
using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CocoDoogy.Tile
{
    public class HexTile: MonoBehaviour
    {
        /// <summary>
        /// Pooling을 위한 prefab 정보
        /// </summary>
        private static HexTile prefab = null;
        
        
        [Header("Object Inner Reference")]
        [Tooltip("Tile Model Transform")] [SerializeField] private Transform modelGroup;
        [Tooltip("기물을 배치하는 Parent Transform")] [SerializeField] private Transform pieceGroup;

        public HexTileData BaseData { get; private set; } = null;
        public HexTileData CurrentData { get; private set; } = null;

        /// <summary>
        /// 해당 타일이 현재 이동 가능 타일인치 확인
        /// </summary>
        public bool IsPlaceable
        {
            get
            {
                Piece.Piece centerPiece = GetPiece(HexDirection.Center);

                bool canTileMove = CurrentData.canMove;
                bool canPieceMove = !centerPiece || centerPiece.BaseData.canMove;
                bool hasBridge = centerPiece &&
                                CurrentData.type is TileType.Water &&
                                centerPiece.BaseData.type is PieceType.Bridge or PieceType.FloatedCrate;
                return canTileMove && canPieceMove || hasBridge;
            }
        }

        public Vector2Int GridPos
        {
            get => gridPos;
            set => transform.position = (gridPos = value).ToWorldPos();
        }
        
        public Piece.Piece[] Pieces { get; } = new Piece.Piece[7];
        public Transform PieceGroup => pieceGroup;
        private Outlinable Outline { get; set; } = null;


        #region Events
        /// <summary>
        /// 기물이 변경됐을 때 동작
        /// </summary>
        public event Action<HexTile, HexDirection> OnPieceChanged = null;
        /// <summary>
        /// 타일이 회전 시작했을 때 동작
        /// </summary>
        public event Action<HexTile, HexRotate> OnRotateChanged = null;
        /// <summary>
        /// 타일이 회전을 완료했을 때 동작
        /// </summary>
        public event Action<HexTile, HexRotate> OnRotateComplete = null;
        #endregion


        private bool hasInit = false;
        private GameObject Model
        {
            get => model;
            set
            {
                Outline = (model = value).GetComponent<Outlinable>();
                if (!Outline) return;
                OffOutline();
            }
        }
        private Vector2Int gridPos = Vector2Int.zero;
        
        private Action<HexTile> onSpawn = null;
        private Action<HexTile> onRelease = null;
        private GameObject model = null;


        #if UNITY_EDITOR
        void Reset()
        {
            modelGroup = transform.Find("Model");
            pieceGroup = transform.Find("Pieces");
        }
        void OnDrawGizmosSelected()
        {
            for (int i = 0; i < 6; i++)
            {
                HexDirection direction = (HexDirection)i;
                Vector3 origin = transform.position + transform.rotation * direction.GetPos() * transform.lossyScale.x;
                
                Debug.DrawLine(origin, origin + Vector3.up, Color.magenta);
            }
        }

        [ContextMenu("Debug")]
        private void CallData()
        {
            StringBuilder sb = new();
            sb.Append($"{BaseData.type}\n");
            for (int i = 0; i < Pieces.Length; i++)
            {
                sb.Append($"{(HexDirection)i} - {(Pieces[i] ? Pieces[i].BaseData.type : PieceType.None)}\n");
            }
            Debug.LogWarning(sb.ToString());
        }
        #endif
        
        void OnDestroy()
        {
            HexTile.RemoveTile(this);
        }


        #region ◇ LifeCycle ◇
        /// <summary>
        /// Pool에 포함되기 전에 첫 생성됐을 때의 초기화 작업
        /// </summary>
        /// <param name="data"></param>
        private void Init(HexTile data)
        {
            hasInit = true;

            GetComponentsInChildren<IInit<HexTile>>().GetEvents()?.Invoke(data);

            onSpawn = GetComponentsInChildren<ISpawn<HexTile>>().GetEvents();
            onRelease = GetComponentsInChildren<IRelease<HexTile>>().GetEvents();
        }

        /// <summary>
        /// Pool에서 Spawn됐을 때 동작
        /// </summary>
        /// <param name="data"></param>
        private void Spawn(HexTileData data)
        {
            HexTile.AddTile(this);
            
            BaseData = CurrentData = data;
            Model = LeanPool.Spawn(BaseData.modelPrefab, modelGroup);
            
            onSpawn?.Invoke(this);
        }
        /// <summary>
        /// <b>HexTileMap</b> 전용<br/>
        /// HexTile 객체를 제거하고, Pool로 반환
        /// </summary>
        public void Release()
        {
            for (int i = 0; i < Pieces.Length; i++)
            {
                if(Pieces[i] == null) continue;
                Pieces[i].Release();
            }
            LeanPool.Despawn(Model);
            
            HexTile.RemoveTile(this);
            
            onRelease?.Invoke(this);
            LeanPool.Despawn(this);
        }
        #endregion


        #region ◇ InGame Functions ◇
        // ReSharper disable Unity.PerformanceAnalysis
        public void Change(TileType type)
        {
            // TODO: 나중에 효과가 추가될 수도 있음
            if (Model)
            {
                LeanPool.Despawn(Model);
            }
            CurrentData = DataManager.GetTileData(type);
            Model = LeanPool.Spawn(CurrentData.modelPrefab, modelGroup);
        }

        public void Rotate(HexRotate rotate)
        {
            DOTween.Kill(this, true);

            Piece.Piece[] prePieces = (Piece.Piece[])Pieces.Clone();
            // Piece들 위치 이동
            for (int i = 0; i < 6; i++)
            {
                HexDirection direction = (HexDirection)i;
                int targetIdx = (int)direction.AddRotate(rotate);
                Pieces[targetIdx] = prePieces[i];
            }

            // Center는 LookDirection 회전만 해줘야 함
            Piece.Piece centerPiece = GetPiece(HexDirection.Center);
            if (centerPiece)
            {
                centerPiece.LookDirection = centerPiece.LookDirection.AddRotate(rotate);
            }
            
            VfxManager.CreateVfx(VfxType.None, transform.position, transform.rotation);
            OnRotateChanged?.Invoke(this, rotate);

            // 회전 효과
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalRotate(new Vector3(0, rotate.ToDegree(), 0), Constants.ROTATE_DURATION));
            sequence.OnComplete(() =>
            {
                transform.rotation = Quaternion.identity;
                for (int i = 0; i < Pieces.Length; i++)
                {
                    var piece = Pieces[i];
                    if (!piece) continue;

                    piece.SetPosition((HexDirection)i);
                }
                OnRotateComplete?.Invoke(this, rotate);
            });
            sequence.SetId(this);
            sequence.Play();
        }

        public void DrawOutline(Color color)
        {
            Outline.SetEnable(true);
            Outline.FrontParameters.Color = color;
            //Outline.FrontParameters.FillPass.SetColor("Basic/Color fill", color);
        }
        public void OffOutline() => Outline.SetEnable(false);
        #endregion


        #region ◇ 기물 관련 기능 ◇
        /// <summary>
        /// 해당 기물을 갖고 있는지 확인
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasPiece(PieceType type, out Piece.Piece findedPiece)
        {
            findedPiece = null;
            foreach (var piece in Pieces)
            {
                if (piece && piece.BaseData.type == type)
                {
                    findedPiece = piece;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 해당 기물을 연결
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public Piece.Piece ConnectPiece(HexDirection direction, Piece.Piece piece)
        {
            var prePiece = GetPiece(direction);
            if (prePiece && prePiece != piece && piece.BaseData.type != PieceType.Bridge)
            {
                RemovePiece(direction);
            }

            Pieces[(int)direction] = piece;
            piece.SetParent(this);
            piece.SetPosition(direction);

            return piece;
        }
        /// <summary>
        /// 해당 위치에 기물 추가
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="pieceType"></param>
        /// <param name="lookDirection"></param>
        /// <returns></returns>
        public Piece.Piece SetPiece(HexDirection direction, PieceType pieceType, HexDirection lookDirection)
        {
            if (pieceType == PieceType.None) // None기물을 배치시도하는 건 제거로 인지
            {
                VfxManager.CreateVfx(VfxType.None, transform.position, transform.rotation);
                RemovePiece(direction);
                return null;
            }

            Piece.Piece result = Piece.Piece.Create(pieceType, this);
            result.LookDirection = lookDirection;
            ConnectPiece(direction, result);
            
            OnPieceChanged?.Invoke(this, direction);
            SfxManager.PlaySfx(SfxType.Gimmick_ObjectSpawn);
            
            return result;
        }
        /// <summary>
        /// 해당 위치에 기물 제거
        /// </summary>
        /// <param name="direction"></param>
        public void RemovePiece(HexDirection direction)
        {
            if (!Pieces[(int)direction]) return;
            
            Pieces[(int)direction].Release();
            Pieces[(int)direction] = null;
            
            OnPieceChanged?.Invoke(this, direction);
            
            SfxManager.PlaySfx(SfxType.Gimmick_ObjectDestroy);
        }
        /// <summary>
        /// 해당 위치의 기물 확인
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Piece.Piece GetPiece(HexDirection direction) => Pieces[(int)direction];
        #endregion
        
        #region ◇ 이동 가능 확인 연쇄책임패턴 ◇
        public List<Vector2Int> CanMovePoses()
        {
            List<Vector2Int> result = new();

            for(int i = 0;i < 6;i++)
            {
                HexDirection direction = (HexDirection)i;
                if(CanMove(direction))
                {
                    result.Add(GridPos.GetDirectionPos(direction));
                }
            }

            return result;
        }
        /// <summary>
        /// 자신을 중심으로 주변 타일이 이동가능한 곳인지 확인
        /// </summary>
        /// <param name="direction">진행 시도 방향</param>
        /// <returns></returns>z
        public bool CanMove(HexDirection direction)
        {
            Vector2Int pos = GridPos.GetDirectionPos(direction);
            HexTile target = HexTile.GetTile(pos);
            HexDirection mirror = direction.GetMirror(); // 확인하는 타일 기준으로, 난 어느 방향인지
            
            // 이동이 불가능한 이유가 존재하는지 확인
            foreach (var check in CanMoveChecks)
            {
                if (check(target, mirror)) return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 해당 타일이 현재 타일에서 갈 수 있는 타일인지 확인하는 책임연쇄패턴
        /// </summary>
        private static readonly Func<HexTile, HexDirection, bool>[] CanMoveChecks =
        {
            HasNotTile,
            HasNotActionPoints,
            CantMoveTile,
            HasMyObstacle,
            HasTargetObstacle,
            CheckMyBridge,
            CheckTargetBridge,
            CheckCrateMovable,
        };

        /// <summary>
        /// 타일이 없는지
        /// </summary>
        private static bool HasNotTile(HexTile hexTile, HexDirection direction) => !hexTile;
        /// <summary>
        /// 이동에 필요한 충분한 행동력을 보유했는지
        /// </summary>
        private static bool HasNotActionPoints(HexTile hexTile, HexDirection direction)
        {
            HexTile myTile = HexTile.GetTile(hexTile.GridPos.GetDirectionPos(direction));
            return myTile.CurrentData.RealMoveCost > InGameManager.ActionPoints;
        }
        /// <summary>
        /// 타일이 이동할 수 있는 타일인지, 그리고 다리조차 없는지
        /// </summary>
        private static bool CantMoveTile(HexTile hexTile, HexDirection direction)
        {
            Piece.Piece centerPiece = hexTile.GetPiece(HexDirection.Center);
            if (centerPiece && hexTile.CurrentData.type == TileType.Water)
            {
                // 다리나, 물 위에 상자가 있으면 일단 건널 수 있는 타일로 인지
                return centerPiece.BaseData.type is not (PieceType.Bridge or PieceType.FloatedCrate);
            }
            
            return !hexTile.IsPlaceable;
        }
        /// <summary>
        /// 이동불가 장애물이 현재 타일에 존재하는지
        /// </summary>
        private static bool HasMyObstacle(HexTile hexTile, HexDirection direction)
        {
            HexTile myTile = HexTile.GetTile(hexTile.GridPos.GetDirectionPos(direction));
            HexDirection myDirection = direction.GetMirror();

            return myTile.Pieces[(int)myDirection] && !myTile.Pieces[(int)myDirection].BaseData.canMove;
        }
        /// <summary>
        /// 이동불가 장애물이 목표 타일에 존재하는지
        /// </summary>
        private static bool HasTargetObstacle(HexTile hexTile, HexDirection direction) =>
            (hexTile.GetPiece(HexDirection.Center) &&                    // 센터에 기물이 존재하고,
            !hexTile.GetPiece(HexDirection.Center).BaseData.canMove) ||  // 움직일 수 없는지
            (hexTile.Pieces[(int)direction] &&                              // 그 방향에 기물이 존재하고,
            !hexTile.Pieces[(int)direction].BaseData.canMove);              // 움직일 수 없는지
        /// <summary>
        /// 내 타일에 다리가 있는지 확인 및 갈 수 있는 방향인지까지
        /// </summary>
        private static bool CheckMyBridge(HexTile hexTile, HexDirection direction)
        {
            HexTile myTile = HexTile.GetTile(hexTile.GridPos.GetDirectionPos(direction));
            Piece.Piece piece = myTile.GetPiece(HexDirection.Center);
            if (piece && piece.BaseData.type == PieceType.Bridge)
            {
                return piece.LookDirection != direction && piece.LookDirection != direction.GetMirror();
            }
            return false;
        }
        /// <summary>
        /// 상대 타일에 다리가 있는지 확인 및 갈 수 있는 방향인지까지
        /// </summary>
        private static bool CheckTargetBridge(HexTile hexTile, HexDirection direction)
        {
            Piece.Piece piece = hexTile.GetPiece(HexDirection.Center);
            if (piece && piece.BaseData.type == PieceType.Bridge)
            {
                return piece.LookDirection != direction && piece.LookDirection != direction.GetMirror();
            }
            return false;
        }
        /// <summary>
        /// 만약 해당 위치에 Crate가 있다면, 움직일 수 있는지
        /// </summary>
        private static bool CheckCrateMovable(HexTile hexTile, HexDirection direction)
        {
            Piece.Piece centerPiece = hexTile.GetPiece(HexDirection.Center);
            if (!centerPiece || centerPiece.BaseData.type is not (PieceType.Crate or PieceType.GravityCrate)) return false; // 없으면 문제 없음
            
            HexTile acrossTile = HexTile.GetTile(hexTile.GridPos.GetDirectionPos(direction.GetMirror()));
            if (!acrossTile /*|| !acrossTile.CurrentData.canMove*/) return true; // 타일 상태 때문에 상자가 건너편으로 갈 수 없음
            
            // 목표 타일의 중간에 발판이 아닌 기물이 존재하는지
            Piece.Piece acrossCenterPiece = acrossTile.GetPiece(HexDirection.Center);
            if (acrossCenterPiece && acrossCenterPiece.BaseData.type != PieceType.GravityButton) return true; // 발판이 아닌 물체가 있으면 갈 수가 없음.

            // 현재 타일의 진행 방향에 장애물 있는지
            HexDirection tilePath = direction.GetMirror();
            bool canTilePath = hexTile.GetPiece(tilePath)?.BaseData.canMove ?? true;
            if(!canTilePath) return true;
            
            // 목표 타일의 진행 방향에 장애물이 있는지
            HexDirection acrossPath = direction;
            bool canAcrossPath = acrossTile.GetPiece(acrossPath)?.BaseData.canMove ?? true;
            if(!canAcrossPath) return true;
            
            return false;
        }
        #endregion


        #region ◇ Tile Manage ◇
        /// <summary>
        /// 현재 존재하는 타일들
        /// </summary>
        public static Dictionary<Vector2Int, HexTile> Tiles { get; } = new();
        
        
        /// <summary>
        /// 해당 위치의 HexTile 반환
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static HexTile GetTile(Vector2Int pos) => Tiles.GetValueOrDefault(pos);
        /// <summary>
        /// 해당 HexTile이 생겼다고 등록 
        /// </summary>
        /// <param name="hexTile"></param>
        public static void AddTile(HexTile hexTile)
        {
            if (!Tiles.TryAdd(hexTile.GridPos, hexTile))
            {
                Debug.LogError("이미 타일이 존재하는 위치");
            }
        }
        /// <summary>
        /// 해당 HexTile이 사라졌다고 해제
        /// </summary>
        /// <param name="hexTile"></param>
        public static void RemoveTile(HexTile hexTile)
        {
            if (!Tiles.ContainsKey(hexTile.GridPos))
            {
                Debug.LogError("타일이 존재하지 않는 위치");
                return;
            }
            Tiles.Remove(hexTile.GridPos);
        }
        #endregion


        #region ◇ Create ◇
        /// <summary>
        /// <b>게임을 실행했을 때 동작</b><br/>
        /// prefab을 Resources에서 호출
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            prefab = Resources.Load<HexTile>("Tile/Tile");
        }

        /// <summary>
        /// HexTile Pool에서 호출
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="gridPos">그리드 상 위치</param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static HexTile Create(TileType tileType, Vector2Int gridPos, Transform parent = null)
        {
            HexTileData data = DataManager.GetTileData(tileType);
            
            return data is null ? null : Create(data, gridPos, parent);
        }
        /// <summary>
        /// HexTile Pool에서 호출
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gridPos">그리드 상 위치</param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static HexTile Create(HexTileData data, Vector2Int gridPos, Transform parent = null)
        {
            HexTile result = LeanPool.Spawn(prefab, parent);
            if (!result.hasInit)
            {
                // Component 연결 등의 초기화를 한 적이 없는 생성된 타일이라면, 초기화
                result.Init(result);
            }
            result.GridPos = gridPos;
            result.Spawn(data);

            return result;
        }
        #endregion
    }
}