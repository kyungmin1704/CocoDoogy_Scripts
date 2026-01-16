using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.UI;
using CocoDoogy.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI
{
    public class WeatherListPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private WeatherButton buttonPrefab;
        [SerializeField] private RectTransform buttonGroup;


        void Start()
        {
            WeatherAddPopup.OnWeatherAdded += OnWeatherAdded;
            MapSaveLoader.OnMapLoaded += OnMapLoaded;
            
            dropdown.AddOptions(new List<string>()
            {
                WeatherType.Sunny.ToString(),
                WeatherType.Rain.ToString(),
                WeatherType.Snow.ToString(),
                WeatherType.Mirage.ToString(),
            });
            dropdown.value = 0;
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        private void OnDestroy()
        {
            WeatherAddPopup.OnWeatherAdded -= OnWeatherAdded;
            MapSaveLoader.OnMapLoaded -= OnMapLoaded;
        }


        private void OnMapLoaded()
        {
            foreach (Transform child in buttonGroup)
            {
                Destroy(child.gameObject);
            }
            
            dropdown.SetValueWithoutNotify((int)HexTileMap.DefaultWeather);
            foreach (var data in HexTileMap.Weathers)
            {
                OnWeatherAdded(data.Key, data.Value);
            }
        }
        private void OnWeatherAdded(int point, WeatherType weather)
        {
            WeatherButton button = Instantiate(buttonPrefab, buttonGroup);
            button.ActionPoint = point;
            button.Weather = weather;
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{point} - {weather}";
        }
        private void OnDropdownValueChanged(int index)
        {
            HexTileMap.DefaultWeather = (WeatherType)index;
        }
    }
}