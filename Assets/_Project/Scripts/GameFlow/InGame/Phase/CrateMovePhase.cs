using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// Player와 Crate가 같은 타일로 있으면 이동 시도<br/>
    /// 그 외 GravityButton Trigger 동작도 수행
    /// </summary>
    public class CrateMovePhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            Vector2Int gridPos = PlayerHandler.GridPos;
            HexTile tile = HexTile.GetTile(gridPos);
            if (!tile) return false; // 타일이 없는 건 경우가 다른 사태임
            
            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            HexDirection moveDir = PlayerHandler.LookDirection;
            if (centerPiece && centerPiece.BaseData.type is PieceType.Crate or PieceType.GravityCrate)
            {
                CommandManager.GimmickPieceMove(gridPos, HexDirection.Center, moveDir);
                SfxManager.PlaySfx(SfxType.Interaction_PushChest);
            }

            centerPiece = tile.GetPiece(HexDirection.Center); // 제거됐을 가능성이 높으므로 새로 받아야 함
            HexTile nextTile = HexTile.GetTile(gridPos.GetDirectionPos(moveDir));
            Piece nextPiece = nextTile ? nextTile.GetPiece(HexDirection.Center) : null;

            PieceType nowCenterType = centerPiece ? centerPiece.BaseData.type : PieceType.None; 
            PieceType nextCenterType = nextPiece ? nextPiece.BaseData.type : PieceType.None; 
            
            if (nowCenterType == PieceType.GravityButton)
            {
                GimmickExecutor.ExecuteFromTrigger(gridPos);
            }
            if (nextCenterType == PieceType.GravityCrate)
            {
                GimmickExecutor.ExecuteFromTrigger(nextTile.GridPos);
            }

            return true;
        }
    }
}