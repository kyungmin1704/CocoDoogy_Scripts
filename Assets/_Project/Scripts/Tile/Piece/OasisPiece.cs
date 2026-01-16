using CocoDoogy.GameFlow.InGame.Weather;
using System;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    [RequireComponent(typeof(Piece))]
    public class OasisPiece: MonoBehaviour
    {
        [SerializeField] private GameObject defaultObject;
        [SerializeField] private GameObject mirageObject;


        void OnEnable()
        {
            WeatherManager.OnWeatherChanged += OnWeatherChanged;
            OnWeatherChanged(WeatherManager.NowWeather);
        }
        void OnDisable()
        {
            WeatherManager.OnWeatherChanged -= OnWeatherChanged;
        }


        private void OnWeatherChanged(WeatherType type)
        {
            defaultObject.SetActive(type != WeatherType.Mirage);
            mirageObject.SetActive(type == WeatherType.Mirage);
        }
    }
}