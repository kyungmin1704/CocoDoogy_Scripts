using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 부두의 배를 되돌려 놓는 Command<br/>
    /// GridPos(Vector2Int), Docked(bool)
    public class DeckResetCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [SerializeField] private Vector2Int gp = Vector2Int.zero;
        [SerializeField] private byte d = 0;
        

        public Vector2Int GridPos { get => gp; private set => gp = value; }
        public bool Docked { get => d == 1; private set => d = value ? (byte)1 : (byte)0; }


        public DeckResetCommand(object param): base(CommandType.DeckReset)
        {
            var data = ((Vector2Int, bool))param;

            GridPos = data.Item1;
            Docked = data.Item2;
        }


        public override void Execute()
        {
            HexTile.GetTile(GridPos).HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = Docked;
        }

        public override void Undo()
        {
            HexTile.GetTile(GridPos).HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = !Docked;
        }
    }
}