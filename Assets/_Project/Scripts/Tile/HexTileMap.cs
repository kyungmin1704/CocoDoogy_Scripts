using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.Tile
{
    /// <summary>
    /// Tile 생성, 제거는 해달 class를 통해서 진행하는 걸 권장
    /// </summary>
    public class HexTileMap : Singleton<HexTileMap>
    {
        /// <summary>
        /// 맵 초기화 가능 회수
        /// </summary>
        public static int RefillPoint { get; set; } = 3;
        /// <summary>
        /// 맵 최대 행동력
        /// </summary>
        public static int ActionPoint { get; set; } = 5;
        
        /// <summary>
        /// 게임 시작점
        /// </summary>
        public static Vector2Int StartPos { get; set; } = Vector2Int.zero;
        /// <summary>
        /// 게임 도착점
        /// </summary>
        public static Vector2Int EndPos { get; set; } = Vector2Int.zero;
        
        /// <summary>
        /// 맵 최소 그리드 위치
        /// </summary>
        public static Vector3 MinPoint { get; set; } = Vector3.zero;
        /// <summary>
        /// 맵 최대 그리드 위치
        /// </summary>
        public static Vector3 MaxPoint { get; set; } = Vector3.zero;

        /// <summary>
        /// 게임 시작 날씨
        /// </summary>
        public static WeatherType DefaultWeather { get; set; } = WeatherType.Sunny;
        
        /// <summary>
        /// Map 내 적용된 기믹들
        /// </summary>
        public static Dictionary<Vector2Int, GimmickData> Gimmicks { get; } = new();
        /// <summary>
        /// Map 내에서 턴에 맞춰 동작할 날씨들
        /// </summary>
        public static Dictionary<int, WeatherType> Weathers { get; } = new();
        

        /// <summary>
        /// 타일을 생성시킬 Parent
        /// </summary>
        [SerializeField] private Transform tileGroup;


        /// <summary>
        /// 타일 생성시킬 parent<br/>
        /// tileGroup이 null이면, 해당 Component가 붙은 transform을 반환
        /// </summary>
        private Transform TileParent => tileGroup ? tileGroup : transform;


        /// <summary>
        /// 해당 GridPos에 존재하는 Gimmick 반환
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public static GimmickData GetGimmick(Vector2Int gridPos) => Gimmicks.GetValueOrDefault(gridPos);
        /// <summary>
        /// 해당 GridPos를 트리거로 사용하는 Gimmick 반환
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public static GimmickData[] GetTriggers(Vector2Int gridPos)
        {
            if (!Instance) return null;

            return Gimmicks.Values.Where(data => data.ContainsTrigger(gridPos)).ToArray();
        }
        /// <summary>
        /// 해당 GridPos에 기믹이 존재하면, 기믹 제거
        /// </summary>
        /// <param name="gridPos"></param>
        public static void RemoveGimmick(Vector2Int gridPos) => Gimmicks.Remove(gridPos);
        
        
        /// <summary>
        /// 타일을 gridPos에 생성 시도
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public static HexTile AddTile(TileType tileType, Vector2Int gridPos)
        {
            if (!Instance) return null;
            
            if (HexTile.Tiles.TryGetValue(gridPos, out HexTile result)) return result;
        
            var tile = HexTile.Create(tileType, gridPos, Instance.TileParent);
            
            return tile;
        }

        /// <summary>
        /// 타일 위에 기물을 추가 시도
        /// </summary>
        /// <param name="selectedTile"></param>
        /// <param name="piece"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Piece.Piece AddPiece(HexTile selectedTile, HexDirection direction, PieceType piece, HexDirection lookDirection = HexDirection.East)
        {
            if (!Instance) return null;
            if (!selectedTile) return null;
            if (piece == PieceType.None) return null;

            Piece.Piece result = selectedTile.GetPiece(direction);
            if (result == null)
            {
                result = selectedTile.SetPiece(direction, piece, lookDirection);
            }
            
            return result;
        }

        /// <summary>
        /// gridPos에 존재하는 Tile 제거 시도
        /// </summary>
        /// <param name="gridPos"></param>
        public static void RemoveTile(Vector2Int gridPos)
        {
            if (!Instance) return;

            HexTile.Tiles.GetValueOrDefault(gridPos)?.Release();
            Gimmicks.Remove(gridPos);
        }
        /// <summary>
        /// 해당 Tile을 제거
        /// </summary>
        /// <param name="selectedTile"></param>
        public static void RemoveTile(HexTile selectedTile) => selectedTile?.Release();

        /// <summary>
        /// 해당 기물을 제거
        /// </summary>
        /// <param name="selectedPiece"></param>
        public static void RemovePiece(HexTile tile, HexDirection direction)
        {
            if (!tile) return;

            tile.RemovePiece(direction);
        }
        
        /// <summary>
        /// 현재 맵 데이터를 초기화
        /// </summary>
        public static void Clear()
        {
            if (!Instance) return;

            PlayerHandler.Clear();
            
            HexTile[] tiles = HexTile.Tiles.Values.ToArray();
            for(int i = 0;i < tiles.Length;i++)
            {
                tiles[i].Release();
            }
            
            StartPos = Vector2Int.zero;
            EndPos = Vector2Int.zero;
            MinPoint = Vector3.zero;
            MaxPoint = Vector3.zero;
            
            Gimmicks.Clear();
            Weathers.Clear();
        }
    }
}
