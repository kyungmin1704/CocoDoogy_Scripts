using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI
{
    public class WeatherAddPopup: MonoBehaviour
    {
        public static event Action<int, WeatherType> OnWeatherAdded = null;
        
        
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private TMP_InputField pointInput;
        [SerializeField] private CommonButton saveButton;
        [SerializeField] private CommonButton cancelButton;


        void Awake()
        {
            dropdown.AddOptions(new List<string>()
            {
                WeatherType.Sunny.ToString(),
                WeatherType.Rain.ToString(),
                WeatherType.Snow.ToString(),
                WeatherType.Mirage.ToString(),
            });
            dropdown.value = 0;
            saveButton.onClick.AddListener(OnSaveButton);
            cancelButton.onClick.AddListener(OnCancelButton);
        }


        private void OnSaveButton()
        {
            if (!int.TryParse(pointInput.text, out int point))
            {
                MessageDialog.ShowMessage("추가 실패", "날씨 포인트는 숫자로 적어야 합니다.", DialogMode.Confirm, null);
                return;
            }
            
            WeatherType weather = (WeatherType)dropdown.value;
            HexTileMap.Weathers.Add(point, weather);
            OnWeatherAdded?.Invoke(point, weather);
            gameObject.SetActive(false);
        }
        private void OnCancelButton()
        {
            gameObject.SetActive(false);
        }
    }
}