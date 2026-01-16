using CocoDoogy.MapEditor.Controller;
using CocoDoogy.MapEditor.GimmickConnector;
using CocoDoogy.MapEditor.UI.GimmickConnector.Effect;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using Lean.Pool;
using System;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI.GimmickConnector
{
    public class GimmickConnectPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown gimmickTypeDropdown;

        [Header("TargetType Pages")]
        [SerializeField] private TextMeshProUGUI tileInfoText;

        [Header("Gimmick Pages")]
        [SerializeField] private GimmickPageBase[] effectPages;

        [Header("Trigger Page")]
        [SerializeField] private GimmickTriggerButton triggerButtonPrefab;
        [SerializeField] private RectTransform triggerButtonGroup;
        [SerializeField] private CommonButton triggerAddButton;

        [Header("Save/Delete Buttons")]
        [SerializeField] private CommonButton saveButton;
        [SerializeField] private CommonButton deleteButton;


        public GimmickData SelectedGimmick { get; private set; } = null;


        void Awake()
        {
            // 초기화가 필요한 자식들 초기화
            foreach (var page in effectPages)
            {
                page.Init(this);
            }

            // GimmickType을 Dropdown에 추가
            gimmickTypeDropdown.options.AddRange(new[]
            {
                new TMP_Dropdown.OptionData(GimmickType.None.ToString()),
                new TMP_Dropdown.OptionData(GimmickType.TileRotate.ToString()),
                new TMP_Dropdown.OptionData(GimmickType.PieceChange.ToString()),
            });

            gimmickTypeDropdown.onValueChanged.AddListener(OnGimmickTypeChanged);
            triggerAddButton.onClick.AddListener(OnTriggerAddButton);
            saveButton.onClick.AddListener(OnSaveButton);
            deleteButton.onClick.AddListener(OnDeleteButton);
        }


        /// <summary>
        /// UI 오픈
        /// </summary>
        /// <param name="tile"></param>
        public void OpenFromTile(Vector2Int gridPos)
        {
            gameObject.SetActive(true);

            SelectedGimmick = HexTileMap.GetGimmick(gridPos);
            if (SelectedGimmick is null) // 기믹이 없던 위치라면
            {
                HexTileMap.Gimmicks.Add(gridPos, SelectedGimmick = new());
                SelectedGimmick.Target.GridPos = gridPos;
            }
            tileInfoText.text = $"{HexTile.GetTile(gridPos).CurrentData.type} {gridPos}";

            SetUI(SelectedGimmick);
        }


        /// <summary>
        /// 기믹 데이터에 맞게 UI 설정
        /// </summary>
        /// <param name="data"></param>
        private void SetUI(GimmickData data)
        {
            gimmickTypeDropdown.SetValueWithoutNotify((int)data.Type);
            OnGimmickTypeChanged((int)data.Type);

            // 기존의 TriggerButton 삭제
            foreach (Transform child in triggerButtonGroup)
            {
                LeanPool.Despawn(child.gameObject);
            }

            // TriggerButton 생성
            foreach (var trigger in data.Triggers)
            {
                GimmickTriggerButton gimmickButton = LeanPool.Spawn(triggerButtonPrefab, triggerButtonGroup);
                gimmickButton.Init(data, trigger);
            }
        }

        private void OpenGimmickTargetPage(GimmickType type)
        {
            foreach (var page in effectPages)
            {
                page.Hide();
            }

            try
            {
                if (type == GimmickType.None)
                {
                    // 적절한 삭제 아이콘을 줘야할 것 같음
                    return;
                }
                effectPages[(int)type - 1].Show();
            }
            catch (IndexOutOfRangeException e)
            {
                // 배열 내에 없는 적합하지 않은 GimmickType일 때
                Debug.LogError(e.Message);
            }
        }

        private bool CanSave()
        {
            if (SelectedGimmick.Type == GimmickType.None)
            {
                MessageDialog.ShowMessage("저장 실패", "기믹 타입을 지정하지 않으면, 저장할 수 없습니다.", DialogMode.Confirm, null);
                return false;
            }
            foreach (var page in effectPages)
            {
                if (!page.gameObject.activeSelf) continue;
                if (!page.IsSuccess) return false;
            }

            return true;
        }


        #region ◇ UI Events ◇
        private void OnGimmickTypeChanged(int idx)
        {
            OpenGimmickTargetPage(SelectedGimmick.Type = (GimmickType)idx);
        }

        private void OnTriggerAddButton()
        {
            if (!CanSave()) return;

            MapEditorController.EditMode = MapEditMode.GimmickTriggerMode;

            gameObject.SetActive(false);
        }

        private void OnSaveButton()
        {
            if (!CanSave()) return;
            // Save라고는 하더라도, 어차피 객체가 UI 인터랙션에 맞춰 실시간으로 바뀌기 때문에 따로 저장 과정을 수반할 필요는 없음.

            gameObject.SetActive(false);
        }
        private void OnDeleteButton()
        {
            HexTileMap.RemoveGimmick(SelectedGimmick.Target.GridPos);

            gameObject.SetActive(false);
        }
        #endregion
    }
}