using CocoDoogy.Data;
using CocoDoogy.MapEditor.Controller;
using CocoDoogy.MapEditor.UI.GimmickConnector;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using CocoDoogy.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MapEditor.UI
{
    public class PieceDeployPanel : MonoBehaviour
    {
        [Header("Gimmick Connector")]
        [SerializeField] private GimmickConnectPanel gimmickPanel;

        [Header("Piece Button Settings")]
        [SerializeField] private PieceButton pieceButtonPrefab;
        [SerializeField] private RectTransform pieceButtonGroup;

        [Header("Slots")]
        [SerializeField] private CommonButton ccwRotateButton;
        [SerializeField] private CommonButton cwRotateButton;
        [SerializeField] private PieceIcon[] slotIcons;

        [Header("Connect UIs")]
        [SerializeField] private CommonButton gimmickButton;
        [SerializeField] private CommonButton targetButton;
        [SerializeField] private TMP_InputField buttonLifeInput;
        [SerializeField] private Toggle deckToggle;


        public static HexTile SelectedTile = null;


        void Awake()
        {
            foreach (PieceType type in DataManager.PieceTypes)
            {
                PieceButton prefab = Instantiate(pieceButtonPrefab, pieceButtonGroup);
                prefab.Init(type, OnPieceSelected);
            }
            ccwRotateButton.onClick.AddListener(OnCCWRotateClicked);
            cwRotateButton.onClick.AddListener(OnCWRotateClicked);
            gimmickButton.onClick.AddListener(OnGimmickButtonClicked);
            targetButton.onClick.AddListener(OnTargetButtonClicked);
            buttonLifeInput.onValueChanged.AddListener(OnButtonLifeChanged);
            deckToggle.onValueChanged.AddListener(OnDeckChanged);
        }
        void OnDestroy()
        {
            ClearEvent();
        }


        /// <summary>
        /// Panel 열기
        /// </summary>
        /// <param name="tile">선택된 타일</param>
        public void Open(HexTile tile)
        {
            if (!(SelectedTile = tile)) return;

            SelectedTile.OnRotateChanged += OnRotateChanged;
            SelectedTile.OnPieceChanged += OnPieceChanged;

            DrawAllPiece();

            gameObject.SetActive(true);
        }
        /// <summary>
        /// Panel 닫기
        /// </summary>
        public void Close()
        {
            ClearEvent();

            gameObject.SetActive(false);
            foreach (var slot in slotIcons)
            {
                slot.SetPiece(null);
            }
        }


        /// <summary>
        /// 선택된 타일과 연결한 Event들 제거
        /// </summary>
        private void ClearEvent()
        {
            if (SelectedTile)
            {
                SelectedTile.OnRotateChanged -= OnRotateChanged;
                SelectedTile.OnPieceChanged -= OnPieceChanged;
            }
        }


        private void OnRotateChanged(HexTile tile, HexRotate rotate)
        {
            DrawAllPiece();
        }
        private void OnPieceChanged(HexTile tile, HexDirection direction)
        {
            DrawPiece(direction);
        }


        /// <summary>
        /// 모든 모서리의 기물 그리기
        /// </summary>
        private void DrawAllPiece()
        {
            if (!SelectedTile) return;

            targetButton.gameObject.SetActive(false);
            for (int i = 0; i < 7; i++)
            {
                DrawPiece((HexDirection)i);
            }
        }
        /// <summary>
        /// 모서리의 기물 그리기
        /// </summary>
        /// <param name="direction">모서리 방향</param>
        private void DrawPiece(HexDirection direction)
        {
            if (!SelectedTile) return;

            // 사용할 데이터 세팅
            Piece piece = SelectedTile.GetPiece(direction);
            PieceIcon slotIcon = slotIcons[(int)direction];
            slotIcon.SetPiece(piece);

            if (piece && piece.BaseData.hasTarget)
            {
                targetButton.gameObject.SetActive(true);
            }

            if (direction != HexDirection.Center) return;
            
            // 버튼 트리거 수명
            bool hasButtonPiece = piece && piece.BaseData.type == PieceType.Button;
            buttonLifeInput.gameObject.SetActive(hasButtonPiece);
            if (hasButtonPiece)
            {
                buttonLifeInput.SetTextWithoutNotify(piece.SpecialData);
            }
            
            // 부두 배 정박
            bool hasDeckPiece = SelectedTile.HasPiece(PieceType.Deck, out Piece deckPiece);
            deckToggle.gameObject.SetActive(hasDeckPiece);
            if (hasDeckPiece)
            {
                deckToggle.SetIsOnWithoutNotify(bool.TryParse(deckPiece.SpecialData, out bool isOn) && isOn);
            }
        }


        /// <summary>
        /// 반시계방향으로 한 번 회전
        /// </summary>
        private void OnCCWRotateClicked()
        {
            SelectedTile.Rotate(HexRotate.CCW60);
        }
        /// <summary>
        /// 시계방향으로 한 번 회전
        /// </summary>
        private void OnCWRotateClicked()
        {
            SelectedTile.Rotate(HexRotate.CW60);
        }
        /// <summary>
        /// 기믹 팝업 오픈
        /// </summary>
        private void OnGimmickButtonClicked()
        {
            gimmickPanel.OpenFromTile(SelectedTile.GridPos);
            Close();
        }
        /// <summary>
        /// 기믹 팝업 오픈
        /// </summary>
        private void OnTargetButtonClicked()
        {
            MapEditorController.EditMode = MapEditMode.PieceTargetMode;
        }
        
        private void OnButtonLifeChanged(string newValue)
        {
            if (!int.TryParse(newValue, out int num)) return;

            Piece centerPiece = SelectedTile.GetPiece(HexDirection.Center);
            centerPiece.SpecialData = newValue;
        }

        private void OnDeckChanged(bool isOn)
        {
            if(!SelectedTile.HasPiece(PieceType.Deck, out Piece piece)) return;
            
            piece.SpecialData = isOn.ToString();
        }

        /// <summary>
        /// 기물 설치
        /// </summary>
        /// <param name="pieceType"></param>
        private void OnPieceSelected(PieceType pieceType)
        {
            PieceData data = DataManager.GetPieceData(pieceType);
            if (data == null) return;
            if (data.posType == PiecePosType.None) return;

            if (data.posType.HasFlag(PiecePosType.Side))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!SelectedTile.Pieces[i])
                    {
                        HexTileMap.AddPiece(SelectedTile, (HexDirection)i, data.type);
                        break;
                    }
                }
            }
            else
            {
                HexTileMap.AddPiece(SelectedTile, HexDirection.Center, data.type);
            }
        }
    }
}