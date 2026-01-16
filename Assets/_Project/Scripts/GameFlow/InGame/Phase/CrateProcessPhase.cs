using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// Crate의 전환 처리
    /// </summary>
    public class CrateProcessPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;
            
            foreach (var tile in HexTile.Tiles.Values)
            {
                Piece centerPiece = tile.GetPiece(HexDirection.Center);
                if (!centerPiece ||
                    centerPiece.BaseData.type is not (PieceType.Crate or PieceType.FloatedCrate)) continue;

                
                TileType tileType = tile.CurrentData.type;
                PieceType pieceType = centerPiece.BaseData.type;
                if (tileType == TileType.Water && pieceType == PieceType.Crate)
                {
                    CommandManager.GimmickPieceChange(tile.GridPos, HexDirection.Center, PieceType.FloatedCrate, pieceType,
                        centerPiece.LookDirection, centerPiece.LookDirection);
                    centerPiece = tile.GetPiece(HexDirection.Center);
                    centerPiece.GetComponent<FloatedCratePiece>().ToMove(PlayerHandler.LookDirection.GetMirror());
                }
                else if (tileType == TileType.Ice && pieceType == PieceType.FloatedCrate)
                {
                    SfxManager.PlaySfx(SfxType.Interaction_Sliding);
                    CommandManager.GimmickPieceChange(tile.GridPos, HexDirection.Center, PieceType.Crate, pieceType,
                        centerPiece.LookDirection, centerPiece.LookDirection);
                }
            }
            

            return true;
        }
    }
}