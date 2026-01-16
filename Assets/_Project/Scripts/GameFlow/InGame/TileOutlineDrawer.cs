using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Tile;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    /// <summary>
    /// 고정적으로 들어가야하는 기믹 등을 탐색하기 위한 Outline
    /// </summary>
    public static class TileOutlineDrawer
    {
        private static readonly Stack<HexTile> filledTiles = new();
        

        public static void Clear() => filledTiles.Clear();
        public static void Draw()
        {
            //#if UNITY_EDITOR
            // 기존에 Outline이 들어간 타일 색 제거 
            while (filledTiles.Count > 0)
            {
                filledTiles.Pop().OffOutline();
            }
            
            /*// 기믹이 존재하는 타일이면, 빨간색으로
            foreach (var data in HexTileMap.Gimmicks.Values)
            {
                HexTile gimmickTile = HexTile.GetTile(data.Target.GridPos);
                gimmickTile.DrawOutline(Color.red);
                filledTiles.Push(gimmickTile);
            }*/

            Vector2Int gridPos = PlayerHandler.GridPos;
            HexTile tile = HexTile.GetTile(gridPos);
            List<Vector2Int> canPoses = tile.CanMovePoses();
            // 갈 수 있는 타일 색칠
            foreach (var canPos in canPoses)
            {
                HexTile canTile = HexTile.GetTile(canPos);
                if (!canTile) continue;

                canTile.DrawOutline(Color.green);
                filledTiles.Push(canTile);
            }

            HexTile destination = HexTile.GetTile(HexTileMap.EndPos);
            destination.DrawOutline(Color.purple);
            filledTiles.Push(destination);
            //#endif
        }
    }
}