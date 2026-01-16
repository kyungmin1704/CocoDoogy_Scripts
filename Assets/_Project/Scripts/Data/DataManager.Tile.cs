using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Data
{
    public partial class DataManager
    {
        private readonly Dictionary<TileType, HexTileData> tiles = new();
        private readonly Dictionary<PieceType, PieceData> pieces = new();
        private readonly Dictionary<WeatherType, WeatherData> weathers = new();

        private readonly Dictionary<Theme, List<StageData>> stageList = new();
        private readonly Dictionary<(Theme, int), StageData> stageDict = new();
        private readonly Dictionary<Theme, int> themeMaxIndex = new();


        [SerializeField] private HexTileData[] tileData;
        [SerializeField] private PieceData[] pieceData;
        [SerializeField] private WeatherData[] weatherData;
        [SerializeField] private StageData[] stageData;


        public static IEnumerable<TileType> TileTypes => Instance?.tiles.Keys;
        public static IEnumerable<PieceType> PieceTypes => Instance?.pieces.Keys;
        public static IEnumerable<WeatherType> WeatherTypes => Instance?.weathers.Keys;

        public static IEnumerable<Theme> Themes => Instance?.stageList.Keys;


        private void InitTileData()
        {
            foreach (var data in tileData)
            {
                tiles.Add(data.type, data);
            }
            foreach (var data in pieceData)
            {
                pieces.Add(data.type, data);
            }
            foreach (var data in weatherData)
            {
                weathers.Add(data.type, data);
            }
            foreach (var data in stageData)
            {
                if (!stageList.ContainsKey(data.theme))
                {
                    stageList.Add(data.theme, new());
                }
                stageList[data.theme].Add(data);
                stageDict.Add((data.theme, data.index), data);

                if(!themeMaxIndex.ContainsKey(data.theme))
                {
                    themeMaxIndex.Add(data.theme, data.index);
                }
                else if(themeMaxIndex[data.theme] < data.index)
                {
                    themeMaxIndex[data.theme] = data.index;
                }
            }
        }


        public static HexTileData GetTileData(TileType type)
        {
            if (!Instance) return null;

            return Instance.tiles.GetValueOrDefault(type);
        }

        public static PieceData GetPieceData(PieceType type)
        {
            if (!Instance) return null;

            return Instance.pieces.GetValueOrDefault(type);
        }

        public static WeatherData GetWeatherData(WeatherType type)
        {
            if (!Instance) return null;

            return Instance.weathers.GetValueOrDefault(type);
        }

        public static List<StageData> GetStageData(Theme theme)
        {
            if (!Instance) return null;

            return Instance.stageList.GetValueOrDefault(theme);
        }
        public static StageData GetStageData(Theme theme, int index)
        {
            if (!Instance) return null;

            return Instance.stageDict.GetValueOrDefault((theme, index));
        }
        public static int GetThemeMaxIndex(Theme theme)
        {
            if(!Instance) return int.MaxValue;

            return Instance.themeMaxIndex.GetValueOrDefault(theme, int.MaxValue);
        }
    }
}