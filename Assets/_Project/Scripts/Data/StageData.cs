using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.Data
{
    [CreateAssetMenu(fileName = "New Stage Data", menuName = "Data/Stage Data")]
    public class StageData : ScriptableObject
    {
        public Theme theme = Theme.None;
        public int index = 0;
        
        public string stageName;
        public TextAsset mapData;

        public int[] starThresholds;
        public HexTileData[] tileData;
        public PieceData[] pieceData;
        public WeatherData[] weatherData;
    }
}
