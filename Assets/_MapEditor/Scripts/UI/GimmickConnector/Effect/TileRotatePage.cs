using CocoDoogy.Tile;
using CocoDoogy.UI.Popup;
using System;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI.GimmickConnector.Effect
{
    public class TileRotatePage : GimmickPageBase
    {
        [SerializeField] private TMP_Dropdown rotateDropdown;


        public override bool IsSuccess
        {
            get
            {
                bool result = SelectedGimmick.Effect.Rotate != HexRotate.None;
                if (!result)
                {
                    MessageDialog.ShowMessage("저장 실패", "회전값을 설정해야 합니다.", DialogMode.Confirm, null);
                }

                return result;
            }
        }


        protected override void OnInit(GimmickConnectPanel parent)
        {
            // 회전 값을 Dropdown에 추가
            rotateDropdown.options.Clear();
            rotateDropdown.options.AddRange(new[]
            {
                new TMP_Dropdown.OptionData(HexRotate.None.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CW60.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CW120.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CW180.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CW240.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CW300.ToString()),

                new TMP_Dropdown.OptionData(HexRotate.CCW60.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CCW120.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CCW180.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CCW240.ToString()),
                new TMP_Dropdown.OptionData(HexRotate.CCW300.ToString()),
            });

            rotateDropdown.onValueChanged.AddListener(OnRotateChanged);
        }

        public override void Show()
        {
            base.Show();

            for (int i = 0; i < rotateDropdown.options.Count; i++)
            {
                if (rotateDropdown.options[i].text == SelectedGimmick.Effect.Rotate.ToString())
                {
                    rotateDropdown.SetValueWithoutNotify(i);
                    break;
                }
            }
        }


        private void OnRotateChanged(int idx)
        {
            HexRotate rotate = Enum.Parse<HexRotate>(rotateDropdown.options[idx].text);
            SelectedGimmick.Effect.Rotate = rotate;
        }
    }
}