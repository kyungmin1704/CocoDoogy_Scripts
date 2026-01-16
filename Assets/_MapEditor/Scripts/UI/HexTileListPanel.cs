using CocoDoogy.Data;
using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI
{
    public class HexTileListPanel: MonoBehaviour
    {
        [SerializeField] private HexTileButton buttonPrefab;
        [SerializeField] private RectTransform buttonGroup;


        void Start()
        {
            foreach (TileType type in DataManager.TileTypes)
            {
                HexTileButton prefab = Instantiate(buttonPrefab, buttonGroup);
                prefab.Init(type);
            }
        }
    }
}