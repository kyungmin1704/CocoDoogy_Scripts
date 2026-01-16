using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect.DetailInfo
{
    public class DetailItem: MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;


        public void Init(HexTileData data)
        {
            Init(data.tileIcon, data.tileName, data.description);
        }
        public void Init(PieceData data)
        {
            Init(data.pieceIcon, data.pieceName, data.description);
        }
        public void Init(WeatherData data)
        {
            Init(data.weatherIcon, data.weatherName, data.description);
        }
        private void Init(Sprite icon, string name, string description)
        {
            iconImage.sprite = icon;
            nameText.text = name;
            descriptionText.text = description;
        }
    }
}