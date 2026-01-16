using CocoDoogy.Data;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using System;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI.GimmickConnector.Effect
{
    public class PieceChangePage : GimmickPageBase
    {
        [SerializeField] private GameObject lookDirectionLine;

        [Header("Dropdowns")]
        [SerializeField] private TMP_Dropdown deployDirectionDropdown;
        [SerializeField] private TMP_Dropdown lookDirecitonDropdown;
        [SerializeField] private TMP_Dropdown pieceDropdown;


        public override bool IsSuccess
        {
            get
            {
                /*bool result = SelectedGimmick.Effect.PieceType != PieceType.None;
                if(!result)
                {
                    MessageDialog.ShowMessage("저장 실패", "설치할 기물을 설정해야 합니다.", DialogMode.Confirm, null);
                }

                return result;*/
                // GimmickType.Destroy를 없애서 이렇게 처리해야 함
                // PieceType.None은 Destroy랑 동일하게 동작함
                return true;
            }
        }



        protected override void OnInit(GimmickConnectPanel parent)
        {
            // 배치할 위치에 대한 걸 정의
            deployDirectionDropdown.options.Clear();
            deployDirectionDropdown.options.AddRange(new[]
            {
                new TMP_Dropdown.OptionData(HexDirection.East.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.NorthEast.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.NorthWest.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.West.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.SouthWest.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.SouthEast.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.Center.ToString()),
            });

            // Center에 경우 바라볼 방향을 정의
            lookDirecitonDropdown.options.Clear();
            lookDirecitonDropdown.options.AddRange(new[]
            {
                new TMP_Dropdown.OptionData(HexDirection.East.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.NorthEast.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.NorthWest.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.West.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.SouthWest.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.SouthEast.ToString()),
                new TMP_Dropdown.OptionData(HexDirection.Center.ToString()),
            });

            // 기물 Dropdown 정의
            pieceDropdown.options.Clear();
            pieceDropdown.options.Add(new TMP_Dropdown.OptionData(PieceType.None.ToString())); // None이라는 기물은 없기에 강제로 추가
            foreach (var piece in DataManager.PieceTypes)
            {
                pieceDropdown.options.Add(new TMP_Dropdown.OptionData(piece.ToString()));
            }

            // UI Event 연결
            deployDirectionDropdown.onValueChanged.AddListener(OnDirectionChanged);
            lookDirecitonDropdown.onValueChanged.AddListener(OnLookDirectionChanged);
            pieceDropdown.onValueChanged.AddListener(OnPieceChanged);
        }

        public override void Show()
        {
            base.Show();

            // 배치할 방향 드롭다운 미리 넣기
            for (int i = 0; i < deployDirectionDropdown.options.Count; i++)
            {
                if (deployDirectionDropdown.options[i].text == SelectedGimmick.Effect.Direction.ToString())
                {
                    deployDirectionDropdown.value = i;
                    break;
                }
            }

            // 바라볼 방향 드롭다운 미리 넣기
            for (int i = 0; i < lookDirecitonDropdown.options.Count; i++)
            {
                if (lookDirecitonDropdown.options[i].text == SelectedGimmick.Effect.LookDirection.ToString())
                {
                    lookDirecitonDropdown.value = i;
                    break;
                }
            }

            // 배치한 기물 미리 넣기
            for (int i = 0; i < pieceDropdown.options.Count; i++)
            {
                if (pieceDropdown.options[i].text == SelectedGimmick.Effect.NextPiece.ToString())
                {
                    pieceDropdown.value = i;
                    break;
                }
            }
        }


        #region ◇ UI Events ◇
        private void OnDirectionChanged(int idx)
        {
            HexDirection direction = Enum.Parse<HexDirection>(deployDirectionDropdown.options[idx].text);
            SelectedGimmick.Effect.Direction = direction;

            lookDirectionLine.SetActive(direction == HexDirection.Center);
        }
        private void OnLookDirectionChanged(int idx)
        {
            HexDirection direction = Enum.Parse<HexDirection>(lookDirecitonDropdown.options[idx].text);
            SelectedGimmick.Effect.LookDirection = direction;
        }
        private void OnPieceChanged(int idx)
        {
            PieceType piece = Enum.Parse<PieceType>(pieceDropdown.options[idx].text);
            SelectedGimmick.Effect.NextPiece = piece;

            print(piece);
        }
        #endregion
    }
}