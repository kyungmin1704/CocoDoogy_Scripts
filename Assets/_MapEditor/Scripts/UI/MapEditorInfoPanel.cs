using CocoDoogy.MapEditor.Controller;
using CocoDoogy.UI;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI
{
    public class MapEditorInfoPanel : MonoBehaviour
    {
        [Tooltip("현재 선택된 모드 확인용 텍스트")][SerializeField] private TextMeshProUGUI editModeText;

        [Header("Common Editor Buttons")]
        [Tooltip("선택 모드 버튼")][SerializeField] private CommonButton selectButton;
        [Tooltip("생성 모드 버튼")][SerializeField] private CommonButton insertButton;
        [Tooltip("삭제 모드 버튼")][SerializeField] private CommonButton deleteButton;

        [Header("Start/End Editor Buttons")]
        [Tooltip("시작 타일 지정 버튼")][SerializeField] private CommonButton startPosButton;
        [Tooltip("끝 타일 지정 버튼")][SerializeField] private CommonButton endPosButton;


        void Awake()
        {
            MapEditorController.OnEditModeChanged += OnEditorModeModeChanged;

            // 조작 연결
            selectButton.onClick.AddListener(OnSelectButton);
            insertButton.onClick.AddListener(OnInsertButton);
            deleteButton.onClick.AddListener(OnDeleteButton);

            startPosButton.onClick.AddListener(OnStartPosButton);
            endPosButton.onClick.AddListener(OnEndPosButton);
        }

        void OnDestroy()
        {
            MapEditorController.OnEditModeChanged -= OnEditorModeModeChanged;
        }


        private void OnSelectButton()
        {
            MapEditorController.EditMode = MapEditMode.SelectMode;
        }
        private void OnInsertButton()
        {
            MapEditorController.EditMode = MapEditMode.InsertMode;
        }
        private void OnDeleteButton()
        {
            MapEditorController.EditMode = MapEditMode.DeleteMode;
        }

        private void OnStartPosButton()
        {
            MapEditorController.EditMode = MapEditMode.StartPosMode;
        }
        private void OnEndPosButton()
        {
            MapEditorController.EditMode = MapEditMode.EndPosMode;
        }


        /// <summary>
        /// 현재 선택된 EditMode를 Text에 작성
        /// </summary>
        /// <param name="newMode"></param>
        public void OnEditorModeModeChanged(MapEditMode newMode) => editModeText.text = newMode.ToString();
    }
}