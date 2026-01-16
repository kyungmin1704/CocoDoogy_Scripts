using CocoDoogy.Audio;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 전에 위치했던 타일에 GravityButton이 존재했다면 Trigger동작
    /// </summary>
    public class PreGravityButtonPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            Vector2Int prePos = PlayerHandler.GridPos.GetDirectionPos(PlayerHandler.LookDirection.GetMirror());
            HexTile tile = HexTile.GetTile(prePos);
            if (!tile) return true;
            
            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            if (!centerPiece || centerPiece.BaseData.type is not PieceType.GravityButton) return true;
            
            GimmickExecutor.ExecuteFromTrigger(prePos);
            SfxManager.PlaySfx(SfxType.Interaction_PressurePlate);

            return true;
        }
    }
}