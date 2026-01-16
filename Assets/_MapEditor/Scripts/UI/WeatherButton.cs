using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.UI;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI
{
    public class WeatherButton: MonoBehaviour
    {
        public int ActionPoint { get; set; } = 0;
        public WeatherType Weather { get; set; } = WeatherType.None;


        void Start()
        {
            GetComponent<CommonButton>().onClick.AddListener(OnClicked);
        }


        private void OnClicked()
        {
            HexTileMap.Weathers.Remove(ActionPoint);
            Destroy(gameObject);
        }
    }
}