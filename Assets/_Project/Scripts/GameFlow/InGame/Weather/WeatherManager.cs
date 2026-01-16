using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace CocoDoogy.GameFlow.InGame.Weather
{
    public class WeatherManager: Singleton<WeatherManager>
    {
        public static event Action<WeatherType> OnWeatherChanged = null;


        public static WeatherType NowWeather
        {
            get => Instance?.nowWeather ?? WeatherType.Sunny;
            set
            {
                if (!Instance) return;
                if (Instance.nowWeather == value) return;
                
                Instance.nowWeather = value;
                SfxManager.StopSfx(SfxType.Weather_Rain);
                if (value == WeatherType.None) return;
                OnWeatherChanged?.Invoke(value);
            }
        }


        private readonly Stack<HexTile> weatheredTiles = new();
        private WeatherType nowWeather = WeatherType.Sunny;



        #if UNITY_EDITOR
        [MenuItem("Debug/Weather/Sunny")]
        private static void TestSunny()
        {
            CommandManager.Weather(WeatherType.Sunny);
        }
        [MenuItem("Debug/Weather/Rain")]
        private static void TestRain()
        {
            CommandManager.Weather(WeatherType.Rain);
        }
        [MenuItem("Debug/Weather/Snow")]
        private static void TestSnow()
        {
            CommandManager.Weather(WeatherType.Snow);
        }
        [MenuItem("Debug/Weather/Mirage")]
        private static void TestMirage()
        {
            CommandManager.Weather(WeatherType.Mirage);
        }
        #endif


        public static void StartGlobalWeather(WeatherType type)
        {
            if (!Instance) return;

            switch (NowWeather = type)
            {
                case WeatherType.Sunny:
                    while (Instance.weatheredTiles.Count > 0)
                    {
                        HexTile tile = Instance.weatheredTiles.Pop();
                        tile.Change(tile.BaseData.type);
                    }
                    break;
                case WeatherType.Rain:
                    foreach(HexTile tile in HexTile.Tiles.Values)
                    {
                        if (tile.BaseData.type != TileType.Dirt) continue;

                        tile.Change(TileType.Mud);
                        Instance.weatheredTiles.Push(tile);
                        // TODO: 진흙으로 교체
                    }
                    break;
                case WeatherType.Snow:
                    foreach(HexTile tile in HexTile.Tiles.Values)
                    {
                        if (tile.BaseData.type == TileType.Snow)
                        {
                            tile.Change(TileType.HeavySnow);
                            Instance.weatheredTiles.Push(tile);
                            // TODO: 폭설로 교체
                        }
                        else if (tile.BaseData.type == TileType.Water)
                        {
                            tile.Change(TileType.Ice);
                            Instance.weatheredTiles.Push(tile);
                            // TODO: 얼음으로 교체
                        }
                    }
                    break;
            }
        }
    }
}