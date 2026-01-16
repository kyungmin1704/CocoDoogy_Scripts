using CocoDoogy.Data;
using CocoDoogy.Tile;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command
{
    public static partial class CommandManager
    {
        public static void Move(HexDirection direction)
        {
            ExecuteCommand(CommandType.Move, direction);
            
            int sandCount = PlayerHandler.SandCount;
            if(HexTile.GetTile(PlayerHandler.GridPos).CurrentData.type == TileType.Sand)
            {
                ExecuteCommand(CommandType.SandCount, (sandCount, sandCount + 1));
            }
            else if (sandCount > 0)
            {
                ExecuteCommand(CommandType.SandCount, (sandCount, 0));
            }
        }
        public static void Trigger(Vector2Int gridPos, bool isUnInteract = false)
        {
            ExecuteCommand(CommandType.Trigger, (gridPos, isUnInteract));
        }
        
        
        public static void Slide(Vector2Int gridPos)
        {
            ExecuteCommand(CommandType.Slide, (PlayerHandler.GridPos, gridPos));
        }
        public static void Teleport(Vector2Int gridPos)
        {
            ExecuteCommand(CommandType.Teleport, (PlayerHandler.GridPos, gridPos));
        }
        public static void Sail(Vector2Int gridPos)
        {
            ExecuteCommand(CommandType.Sail, (PlayerHandler.GridPos, gridPos));
        }

        
        
        public static void Deploy(Vector2Int gridPos, HexDirection direction)
        {
            ExecuteCommand(CommandType.Deploy, (gridPos, direction));
        }
        public static void Refill()
        {
            foreach(var piece in Piece.Pieces)
            {
                DeckPiece deck = piece.GetComponent<DeckPiece>();
                if(!deck) continue;
                if(deck.IsDocked == deck.PreDocked) continue;

                ExecuteCommand(CommandType.DeckReset, (piece.Parent.GridPos, deck.PreDocked));
            }

            ExecuteCommand(CommandType.Refill, (InGameManager.ActionPoints, PlayerHandler.GridPos, PlayerHandler.SandCount));
            Weather(HexTileMap.DefaultWeather);
        }
        
        public static void Weather(WeatherType weather)
        {
            ExecuteCommand(CommandType.Weather, (WeatherManager.NowWeather, weather));
        }
        public static void GimmickTileRotate(Vector2Int gridPos, HexRotate rotate, bool didGimmicked = false)
        {
            ExecuteCommand(CommandType.Gimmick, (gridPos, GimmickType.TileRotate, (int)rotate, 0, 0, HexDirection.East, HexDirection.East, didGimmicked));
        }
        public static void GimmickPieceChange(Vector2Int gridPos, HexDirection direction, PieceType newPiece, PieceType oldPiece, HexDirection lookDirection, HexDirection preLookDirection, bool didGimmicked = false)
        {
            ExecuteCommand(CommandType.Gimmick, (gridPos, GimmickType.PieceChange, (int)direction, (int)newPiece, (int)oldPiece, lookDirection, preLookDirection, didGimmicked));
        }
        public static void GimmickPieceMove(Vector2Int gridPos, HexDirection pieceDir, HexDirection moveDir, bool didGimmicked = false)
        {
            ExecuteCommand(CommandType.Gimmick, (gridPos, GimmickType.PieceMove, (int)pieceDir, 0, 0, moveDir, moveDir.GetMirror(), didGimmicked));
        }
        
        public static void Regen(int regen)
        {
            ExecuteCommand(CommandType.Increase, regen);
        }

        public static void MaxUp(ItemEffect effect)
        {
            ExecuteCommand(CommandType.MaxUp, effect);
        }

        public static void Recover(ItemEffect effect)
        {
            ExecuteCommand(CommandType.Recover, effect);
        }

        public static void Undo(ItemEffect effect)
        {
            ExecuteCommand(CommandType.Undo, effect);
        }
    }
}