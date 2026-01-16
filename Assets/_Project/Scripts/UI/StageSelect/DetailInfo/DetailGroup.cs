using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect.DetailInfo
{
    public class DetailGroup: MonoBehaviour
    {
        [SerializeField] private DetailItem prefab;
        [SerializeField] private RectTransform groupContent;
        
        
        public void Clear()
        {
            foreach (RectTransform child in groupContent)
            {
                Destroy(child.gameObject);
            }
            gameObject.SetActive(false);
        }

        public void Check()
        {
            if (groupContent.childCount > 0)
            {
                gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(groupContent);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void AddItem(HexTileData data)
        {
            Instantiate(prefab, groupContent).Init(data);
        }
        public void AddItem(PieceData data)
        {
            Instantiate(prefab, groupContent).Init(data);
        }
        public void AddItem(WeatherData data)
        {
            Instantiate(prefab, groupContent).Init(data);
        }
    }
}