using CocoDoogy.Tile.Piece;
using System;
using UnityEngine.Serialization;

namespace CocoDoogy.Tile.Gimmick.Data
{
    [System.Serializable]
    public class GimmickEffect
    {
        public HexRotate Rotate = HexRotate.None;
        public HexDirection Direction = HexDirection.East;
        public HexDirection LookDirection = HexDirection.East;
        public PieceType NextPiece = PieceType.None;
        /// <summary>
        /// Map Load시에 갱신되는 필드
        /// </summary>
        [NonSerialized] public PieceType PrePiece = PieceType.None;
        /// <summary>
        /// Map Load시에 갱신되는 필드
        /// </summary>
        [NonSerialized] public HexDirection PreLookDirection = HexDirection.East;
    }
}