using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Tile.Piece.Trigger;
using UnityEngine;

namespace CocoDoogy.Tile.Gimmick
{
    public static class GimmickExecutor
    {
        public static void ExecuteFromTrigger(Vector2Int gridPos)
        {
            TriggerPieceBase triggerPiece = HexTile.GetTile(gridPos).GetPiece(HexDirection.Center)?.GetComponent<TriggerPieceBase>();
            GimmickData[] gimmicks = HexTileMap.GetTriggers(gridPos);
            if (gimmicks.Length <= 0) return;

            foreach (GimmickData gimmick in gimmicks)
            {
                gimmick.GetTrigger(gridPos).IsTriggered = triggerPiece.IsOn;
                    
                bool gimmickOn = true;
                foreach (var trigger in gimmick.Triggers)
                {
                    if (trigger.IsTriggered == trigger.IsReversed)
                    {
                        gimmickOn = false;
                        break;
                    }
                }

                Debug.Log($"{gimmick.IsOn} -> {gimmickOn}");
                if (gimmick.IsOn == gimmickOn) continue;

                HexTile target = HexTile.GetTile(gimmick.Target.GridPos);
                switch(gimmick.Type)
                {
                    case GimmickType.TileRotate:
                        HexRotate rotate = gimmickOn ? gimmick.Effect.Rotate : gimmick.Effect.Rotate.GetMirror();
                        CommandManager.GimmickTileRotate(target.GridPos, rotate, true);
                        break;
                    case GimmickType.PieceChange:
                        HexDirection direction = gimmick.Effect.Direction;
                        PieceType newPiece = gimmickOn ? gimmick.Effect.NextPiece : gimmick.Effect.PrePiece;
                        PieceType oldPiece = gimmickOn ? gimmick.Effect.PrePiece : gimmick.Effect.NextPiece;
                        HexDirection lookDirection = gimmickOn ? gimmick.Effect.LookDirection : gimmick.Effect.PreLookDirection;
                        HexDirection preLookDirection = gimmickOn ? gimmick.Effect.PreLookDirection : gimmick.Effect.LookDirection;
                        CommandManager.GimmickPieceChange(target.GridPos, direction, newPiece, oldPiece, lookDirection, preLookDirection, true);
                        break;
                }
            }
        }
    }
}