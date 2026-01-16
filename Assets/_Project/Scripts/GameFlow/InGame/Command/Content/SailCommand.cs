using CocoDoogy.Audio;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class SailCommand: CommandBase
    {
        public override bool IsUserCommand => true;


        [UnityEngine.SerializeField] private Vector2Int pp = Vector2Int.zero;
        [UnityEngine.SerializeField] private Vector2Int np = Vector2Int.zero;
        
        
        /// <summary>
        /// 이전 위치
        /// </summary>
        public Vector2Int PrePos { get => pp; private set => pp = value; }

        /// <summary>
        /// 다음 위치
        /// </summary>
        public Vector2Int NextPos { get => np; private set => np = value; }


        public SailCommand(object param) : base(CommandType.Sail)
        {
            var data = ((Vector2Int, Vector2Int))param;
            PrePos = data.Item1;
            NextPos = data.Item2;
        }


        public override void Execute()
        {
            Debug.Log($"{NextPos} - {PrePos}");
            PlayerHandler.Deploy(NextPos);
            InGameManager.ConsumeActionPoint(1);
            SfxManager.PlaySfx(SfxType.Gimmick_DockEnter);

            // 출발지 정리
            HexTile tile = HexTile.GetTile(PrePos);
            tile.HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = false;
            // 도착지 정리
            tile = HexTile.GetTile(NextPos);
            if (tile.HasPiece(PieceType.Deck, out Piece destoPiece))
            {
                destoPiece.GetComponent<DeckPiece>().IsDocked = true;
            }
            
            InGameManager.UseActionPoints++;
        }

        public override void Undo()
        {
            Debug.Log($"{NextPos} - {PrePos}");
            PlayerHandler.Deploy(PrePos);
            InGameManager.RegenActionPoint(1);

            // 출발지 정리
            HexTile tile = HexTile.GetTile(NextPos);
            if (tile.HasPiece(PieceType.Deck, out Piece destoPiece))
            {
                destoPiece.GetComponent<DeckPiece>().IsDocked = false;
            }
            // 도착지 정리
            tile = HexTile.GetTile(PrePos);
            tile.HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = true;
            
            InGameManager.UseActionPoints--;
        }
    }
}