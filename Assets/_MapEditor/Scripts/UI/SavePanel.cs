using CocoDoogy.MapEditor.Controller;
using CocoDoogy.Tile;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using CocoDoogy.Utility;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI
{
    public class SavePanel : MonoBehaviour
    {
        private const string FOLDER_PATH = "Assets/_Project/Data/StageData/MapData/";


        [Header("UI")]
        [SerializeField] private TMP_InputField fileNameInput;
        [SerializeField] private CommonButton saveButton;

        [SerializeField] private TMP_InputField refillCountInput;
        [SerializeField] private TMP_InputField actionPointInput;

        [Header("Buttons")]
        [SerializeField] private CommonButton loadButtonPrefab;
        [SerializeField] private RectTransform loadButtonGroup;


        private string FileName
        {
            get => fileNameInput.text.Trim();
            set => fileNameInput.text = value;
        }


        void Awake()
        {
            RefreshFileInfos();
            saveButton.onClick.AddListener(OnSaveMap);
        }

        void OnEnable()
        {
            MapEditorInputHandler.OnSave += OnSaveMap;
        }
        void OnDisable()
        {
            MapEditorInputHandler.OnSave -= OnSaveMap;
        }


        private void OnSaveMap()
        {
            if (FileName.Length <= 1)
            {
                MessageDialog.ShowMessage("저장 실패", "파일명이 최소 2자리 이상 돼야 합니다.", DialogMode.Confirm, null);
                return;
            }

            string refillCountStr = refillCountInput.text.Trim();
            string actionPointStr = actionPointInput.text.Trim();
            if (!int.TryParse(refillCountStr, out int refillCount))
            {
                MessageDialog.ShowMessage("저장 실패", "초기화 횟수가 숫자가 아닙니다.", DialogMode.Confirm, null);
                return;
            }
            if (!int.TryParse(actionPointStr, out int actionPoint))
            {
                MessageDialog.ShowMessage("저장 실패", "행동력이 숫자가 아닙니다.", DialogMode.Confirm, null);
                return;
            }

            HexTileMap.RefillPoint = refillCount;
            HexTileMap.ActionPoint = actionPoint;

            Directory.CreateDirectory(FOLDER_PATH);
            string path = Path.Combine(FOLDER_PATH, $"{FileName}.json");
            string json = MapSaveLoader.ToJson();
            File.WriteAllText(path, json, Encoding.UTF8);

            Debug.Log($"저장완료 - {path}");

            RefreshFileInfos();
        }

        private void LoadMap(string fileName)
        {
            string path = Path.Combine(FOLDER_PATH, $"{fileName}.json");
            string json = File.ReadAllText(path, Encoding.UTF8);
            MapSaveLoader.Apply(json);

            FileName = fileName;
            refillCountInput.text = HexTileMap.RefillPoint.ToString();
            actionPointInput.text = HexTileMap.ActionPoint.ToString();
        }

        private void RefreshFileInfos()
        {
            foreach (Transform child in loadButtonGroup)
            {
                Destroy(child.gameObject);
            }

            FileInfo[] files = Directory.CreateDirectory(FOLDER_PATH).GetFiles("*.json", SearchOption.TopDirectoryOnly);
            foreach (FileInfo fileInfo in files)
            {
                CommonButton button = Instantiate(loadButtonPrefab, loadButtonGroup);

                int lastIdx = fileInfo.Name.LastIndexOf('.');
                string fileName = fileInfo.Name.Substring(0, lastIdx);
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

                text.text = fileName;
                button.onClick.AddListener(() =>
                {
                    string fName = fileName;
                    LoadMap(fName);

                    // MapEditorController에서도 사용하지만, 별도로 연결하는 노력을 없애는 게 더 나아 보임
                    foreach (var gPos in HexTileMap.Gimmicks.Keys)
                    {
                        HexTile.GetTile(gPos).DrawOutline(Color.blue);
                    }
                });
            }
        }
    }
}
