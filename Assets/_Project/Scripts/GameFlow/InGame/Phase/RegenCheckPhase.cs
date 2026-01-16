using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.MiniGame;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    // TODO: 임시 Phase
    public class RegenCheckPhase : IPhase, IClearable
    {
        private Vector2Int gridPos = Vector2Int.zero;


        public void OnClear()
        {
            gridPos = Vector2Int.zero;
        }

        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            Piece centerPiece = HexTile.GetTile(PlayerHandler.GridPos)?.GetPiece(HexDirection.Center);
            PieceType pieceType = centerPiece?.BaseData.type ?? PieceType.None;

            gridPos = PlayerHandler.GridPos;
            if (pieceType == PieceType.House)
            {
                Debug.Log("OnPhase호출");
                MiniGameManager.OpenRandomGame(IncreaseActionPoints);
                return false;
            }

            if (pieceType is not (PieceType.Field or PieceType.Oasis)) return true;
            if (pieceType is PieceType.Oasis && WeatherManager.NowWeather != WeatherType.Mirage) return true;

            if (centerPiece.BaseData.type is PieceType.Field)
            {
                SfxManager.PlaySfx(SfxType.Item_Recovery);
            }
            else if (centerPiece.BaseData.type is PieceType.Oasis)
            {
                SfxManager.PlaySfx(SfxType.Gimmick_OasisEnter);
            }
            IncreaseActionPoints();


            return true;
        }

        private void IncreaseActionPoints()
        {
            Piece centerPiece = HexTile.GetTile(PlayerHandler.GridPos)?.GetPiece(HexDirection.Center);
            CommandManager.Regen(centerPiece.BaseData.type == PieceType.House ? 2 : 1);
            CommandManager.GimmickPieceChange(PlayerHandler.GridPos, HexDirection.Center, PieceType.None,
                centerPiece.BaseData.type, centerPiece.LookDirection, centerPiece.LookDirection);

            InGameManager.ProcessPhase();
        }
    }
}