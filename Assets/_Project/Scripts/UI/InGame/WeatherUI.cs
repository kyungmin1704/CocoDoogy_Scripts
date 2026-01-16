using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame.Weather;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.InGame
{
    /// <summary>
    /// InGame 상단의 현재 날씨 Icon 표시기
    /// </summary>
    public class WeatherUI : MonoBehaviour
    {
        [SerializeField] private Image weatherImage;


        void OnEnable()
        {
            WeatherManager.OnWeatherChanged += OnWeatherChanged;
        }
        void OnDisable()
        {
            WeatherManager.OnWeatherChanged -= OnWeatherChanged;
        }


        private void OnWeatherChanged(WeatherType type)
        {
            Sprite weatherIcon = null;
            try
            {
                weatherIcon = DataManager.GetWeatherData(type).weatherIcon;
            }
            catch
            {

            }
            weatherImage.sprite = weatherIcon;
        }
    }
}