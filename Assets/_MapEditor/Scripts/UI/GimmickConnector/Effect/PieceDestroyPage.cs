using CocoDoogy.Tile;
using System;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI.GimmickConnector.Effect
{
    public class PieceDestroyPage : GimmickPageBase
    {
        [SerializeField] private TMP_Dropdown directionDropdown;


        public override bool IsSuccess => true;



        protected override void OnInit(GimmickConnectPanel parent)
        {
            // 배치할 위치에 대한 걸 정의
            directionDropdown.options.Clear();
            directionDropdown.options.AddRange(new[]
            {
                new TMP_Dropdown.OptionData(HexDirection.East.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.NorthEast.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.NorthWest.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.West.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.SouthWest.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.SouthEast.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.Center.ToString()),
            });

            directionDropdown.onValueChanged.AddListener(OnDirectionChanged);
        }

        public override void Show()
        {
            base.Show();

            // 배치할 방향 드롭다운 미리 넣기
            for (int i = 0; i < directionDropdown.options.Count; i++)
            {
                if (directionDropdown.options[i].text == SelectedGimmick.Effect.Direction.ToString())
                {
                    directionDropdown.SetValueWithoutNotify(i);
                    break;
                }
            }
        }


        #region ◇ UI Events ◇
        private void OnDirectionChanged(int idx)
        {
            HexDirection direction = Enum.Parse<HexDirection>(directionDropdown.options[idx].text);
            SelectedGimmick.Effect.Direction = direction;
        }
        #endregion
    }
}