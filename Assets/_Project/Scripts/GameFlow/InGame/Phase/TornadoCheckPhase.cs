using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 현재 타일이 토네이도인지 체크
    /// </summary>
    public class TornadoCheckPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            if (!centerPiece || centerPiece.BaseData.type != PieceType.Tornado) return true;
            if (centerPiece.Target == null) return true;

            CommandManager.Teleport((Vector2Int)centerPiece.Target);
            
            return false;
        }
    }
}