using System;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    [CreateAssetMenu(menuName = "Data/Piece Data", fileName = "New Piece Data")]
    public class PieceData: ScriptableObject
    {
        [Header("Common Data")]
        [Tooltip("Piece의 고유 Type")] public PieceType type = PieceType.None;
        [Tooltip("Piece 아이콘")] public Sprite pieceIcon = null;
        [Tooltip("Piece 명칭")] public string pieceName = string.Empty;
        [Tooltip("Tile 설명")][TextArea(3, 10)] public string description = string.Empty;
        [Tooltip("Piece의 시각적 표시용 GameObject")] public Piece modelPrefab = null;
        
        [Header("Piece Setting")]
        [Tooltip("Piece 기본 이동 가능 여부")] public PiecePosType posType = PiecePosType.None;
        [Tooltip("Piece 기본 이동 가능 여부")] public bool canMove = true;
        [Tooltip("Piece 기본 이동 가능 여부")] public bool hasTarget = false;
        
        [Header("Editor")]
        [Tooltip("비고")] [TextArea(3, 10)] [SerializeField] private string editorDescription = string.Empty;


        /// <summary>
        /// 그 위치에 설치 가능한 기물인지 체크
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CanPlace(HexDirection direction) =>
            HexDirection.Center == direction && posType.HasFlag(PiecePosType.Center) ||
            HexDirection.Center != direction && posType.HasFlag(PiecePosType.Side);
    }
}