using CocoDoogy.Data;
using CocoDoogy.LifeCycle;
using CocoDoogy.Tile.Piece.Trigger;
using DG.Tweening;
using Lean.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    public class Piece: MonoBehaviour
    {
        public static readonly List<Piece> Pieces = new();


        public PieceData BaseData { get; private set; } = null;
        public HexTile Parent { get; private set; } = null;
        public HexDirection DirectionPos { get; private set; } = HexDirection.East;
        /// <summary>
        /// 현재 Piece가 Center Piece라면, 바라보는 방향
        /// </summary>
        public HexDirection LookDirection { get; set; } = HexDirection.East;
        public Vector2Int? Target { get; set; } = null;
        /// <summary>
        /// 특수한 기물을 위한 데이터
        /// </summary>
        public string SpecialData
        {
            get => specialData;
            set
            {
                specialData = value;
                onDataInsert?.Invoke(specialData);
            }
        }

        public bool IsTrigger => BaseData &&
            BaseData.type is PieceType.Lever or PieceType.Button or
            PieceType.GravityButton or PieceType.GravityCrate;
        
        
        /// <summary>
        /// 부모가 정해졌을 때 동작
        /// </summary>
        private Action<Piece> onSpawn = null;
        /// <summary>
        /// 릴리즈시 동작
        /// </summary>
        private Action<Piece> onRelease = null;
        
        /// <summary>
        /// 데이터 입력
        /// </summary>
        private Action<string> onDataInsert = null;
        /// <summary>
        /// 특수행동
        /// </summary>
        private Action onExecute = null;

        private bool hasInit = false;
        private string specialData = string.Empty;
        
        
        #region ◇ LifeCycle ◇
        void OnDestroy()
        {
            Pieces.Remove(this);
        }


        private void Init(Piece data)
        {
            hasInit = true;
            GetComponentsInChildren<IInit<Piece>>().GetEvents()?.Invoke(data);
            
            onSpawn = GetComponentsInChildren<ISpawn<Piece>>().GetEvents();
            onRelease = GetComponentsInChildren<IRelease<Piece>>().GetEvents();
            onDataInsert = GetComponentsInChildren<ISpecialPiece>().GetInserts();
            onExecute = GetComponentsInChildren<ISpecialPiece>().GetExecutes();
        }

        private void Spawn(PieceData data)
        {
            BaseData = data;
            Pieces.Add(this);
        }
        public void Release()
        {
            int idx = (int)DirectionPos;
            
            Parent.Pieces[idx] = null;
            Parent = null;
            Target = null;
            
            Pieces.Remove(this);
            onRelease?.Invoke(this);
            LeanPool.Despawn(this);
        }
        #endregion


        /// <summary>
        /// 다른 Tile로 이동
        /// </summary>
        /// <param name="direction">현재 타일에서 움직일 타일의 상대 방향</param>
        public void Move(HexDirection direction)
        {
            HexTile tile = HexTile.GetTile(Parent.GridPos.GetDirectionPos(direction));
            if (!tile) return;

            DOTween.Kill(this, true);

            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            PieceType centerType =  centerPiece ? centerPiece.BaseData.type : PieceType.None;
            if (BaseData.type == PieceType.Crate && centerType == PieceType.GravityButton)
            {
                MoveToGravityButton(tile, direction);
            }
            else if (BaseData.type == PieceType.GravityCrate)
            {
                if (centerType == PieceType.GravityButton)
                {
                    MoveButtonToButton(tile, direction);
                }
                else
                {
                    MoveFromGravityButton(tile, direction);
                }
            }
            else
            {
                MoveDefault(tile, direction);
            }
        }

        /// <summary>
        /// 일반적인 기물 이동
        /// </summary>
        private void MoveDefault(HexTile nextTile, HexDirection direction)
        {
            Vector3 prePos = Parent.GridPos.ToWorldPos() + DirectionPos.GetPos();
            Parent.Pieces[(int)DirectionPos] = null;
            nextTile.ConnectPiece(DirectionPos, this);
            
            // ConnectPiece 단에서 이미 위치를 이동해버리기 때문에 움직이는 효과를 주기 위해
            // 기존의 위치로 강제로 이동을 해줘야 함.
            transform.position = prePos;
            transform.DOMove(nextTile.GridPos.ToWorldPos() + DirectionPos.GetPos(), Constants.MOVE_DURATION)
                .SetId(this);
        }
        /// <summary>
        /// 상자 기물이 발판으로 이동
        /// </summary>
        private void MoveToGravityButton(HexTile nextTile, HexDirection direction)
        {
            Piece piece = nextTile.SetPiece(HexDirection.Center, PieceType.GravityCrate, LookDirection);
            piece.GetComponent<GravityCrate>().ToMove(direction.GetMirror());
            Release();
        }
        /// <summary>
        /// 상자 기물이 발판에서 이동
        /// </summary>
        private void MoveFromGravityButton(HexTile nextTile, HexDirection direction)
        {
            Piece piece = nextTile.SetPiece(HexDirection.Center, PieceType.Crate, LookDirection);
            piece.transform.position = Parent.GridPos.ToWorldPos() + DirectionPos.GetPos();
            piece.transform.DOMove(nextTile.GridPos.ToWorldPos(), Constants.MOVE_DURATION).SetId(piece);
            Parent.SetPiece(HexDirection.Center, PieceType.GravityButton, LookDirection);
        }
        /// <summary>
        /// 상자 기물이 발판에서 발판으로 이동
        /// </summary>
        private void MoveButtonToButton(HexTile nextTile, HexDirection direction)
        {
            Piece piece = nextTile.SetPiece(HexDirection.Center, PieceType.GravityCrate, LookDirection);
            piece.GetComponent<GravityCrate>().ToMove(direction.GetMirror());
            Parent.SetPiece(HexDirection.Center, PieceType.GravityButton, LookDirection);
        }


        /// <summary>
        /// Tile 내의 현재 위치 지정과 함께 위치 지정
        /// </summary>
        /// <param name="direction">Tile 내 위치</param>
        public void SetPosition(HexDirection direction)
        {
            transform.localPosition = (DirectionPos = direction).GetPos();

            if (DirectionPos == HexDirection.Center) // Center Piece는 바라보는 방향 계산식이 다름
            {
                HexRotate rotate = (HexRotate)LookDirection;
                transform.rotation = Quaternion.Euler(0, -(int)rotate * 60, 0);
            }
            else
            {
                transform.LookAt(Parent.transform, Vector3.up);
            }
        }
        
        /// <summary>
        /// 내 부모인 Tile을 지정<br/>
        /// <b>HexTile.ConnectPiece에서만 동작시켜야 함</b>
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(HexTile parent)
        {
            transform.parent = parent.PieceGroup;
            Parent = parent;
            
            onSpawn?.Invoke(this);
        }

        public void Execute()
        {
            onExecute?.Invoke();
        }
        
        
        public static Piece Create(PieceType pieceType, HexTile parent)
        {
            PieceData data = DataManager.GetPieceData(pieceType);
            
            return data is null ? null : Create(data, parent);
        }
        public static Piece Create(PieceData data, HexTile parent)
        {
            Piece result = LeanPool.Spawn(data.modelPrefab, parent.PieceGroup);
            if (!result.hasInit)
            {
                // Component 연결 등의 초기화를 한 적이 없는 생성된 타일이라면, 초기화
                result.Init(result);
            }
            result.Spawn(data);

            return result;
        }
    }
}