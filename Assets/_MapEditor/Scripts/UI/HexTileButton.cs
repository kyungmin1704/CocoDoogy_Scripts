using CocoDoogy.Data;
using CocoDoogy.MapEditor.Controller;
using CocoDoogy.Tile;
using CocoDoogy.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MapEditor.UI
{
    public class HexTileButton : MonoBehaviour
    {
        [SerializeField] private Image tileImage;
        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private CommonButton button;


        private HexTileData hexTileData = null;


        void Reset()
        {
            tileImage = GetComponentInChildren<Image>();
            nameText = GetComponentInChildren<TextMeshProUGUI>();

            button = GetComponentInChildren<CommonButton>();
        }


        public void Init(TileType tileType)
        {
            hexTileData = DataManager.GetTileData(tileType);
            if (hexTileData == null)
            {
                Debug.LogError("Tile Data Not Found.");
                return;
            }

            tileImage.sprite = hexTileData.tileIcon;
            nameText?.SetText(hexTileData.tileName);
            button.onClick.AddListener(OnSelectButton);
        }


        private void OnSelectButton()
        {
            if (MapEditorController.EditMode != MapEditMode.InsertMode)
            {
                MapEditorController.EditMode = MapEditMode.InsertMode;
            }

            MapEditorController.SelectedTileType = hexTileData.type;
        }
    }
}