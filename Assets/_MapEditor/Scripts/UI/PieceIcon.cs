using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using CocoDoogy.UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MapEditor.UI
{
    public class PieceIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private static PieceIcon DragStartIcon = null;
        private static PieceIcon MouseOveredIcon = null;


        [SerializeField] private Image slotImage;
        [SerializeField] private Image pieceImage;
        [SerializeField] private HexDirection direction;


        private Piece selectedPiece = null;


        void Reset()
        {
            slotImage = GetComponent<Image>();
            pieceImage = transform.GetChild(0).GetComponent<Image>();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!selectedPiece) return;
            HexTileMap.RemovePiece(selectedPiece.Parent, direction);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOveredIcon = this;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (MouseOveredIcon == this)
            {
                MouseOveredIcon = null;
            }
        }

        public void OnDrag(PointerEventData eventData) { }
        public void OnBeginDrag(PointerEventData eventData)
        {
            DragStartIcon = this;
            slotImage.color = Color.red;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            // TODO: 코드 좀 더 깔끔하게 만들어야 함.
            PieceIcon startIcon = DragStartIcon;
            PieceIcon endIcon = MouseOveredIcon;
            DragStartIcon = MouseOveredIcon = null;

            if (startIcon)
            {
                startIcon.slotImage.color = Color.white;
            }

            if (!startIcon || !endIcon) return;

            Piece startPiece = startIcon.selectedPiece;
            Piece endPiece = endIcon.selectedPiece;

            if (!startPiece && !endPiece) return;
            if (startPiece == endPiece) return;

            if (!startPiece && !endPiece) return; // 둘 다 null이면 교환 불가능

            bool canStartChange = !startPiece || startPiece.BaseData.CanPlace(endIcon.direction);
            bool canEndChange = !endPiece || endPiece.BaseData.CanPlace(startIcon.direction);

            if (!canStartChange || !canEndChange)
            {
                if (!canStartChange)
                {
                    MessageDialog.ShowMessage("자리 변경 실패",
                        $"{startPiece.BaseData.type}은 {endIcon.direction}에 위치할 수 없습니다.",
                        DialogMode.Confirm, null);
                }
                if (!canEndChange)
                {
                    MessageDialog.ShowMessage("자리 변경 실패",
                        $"{endPiece.BaseData.type}은 {startIcon.direction}에 위치할 수 없습니다.",
                        DialogMode.Confirm, null);
                }
                return;
            }

            HexTile tile = startPiece ? startPiece.Parent : endPiece.Parent;
            PieceType toStart = endPiece ? endPiece.BaseData.type : PieceType.None;
            PieceType toEnd = startPiece ? startPiece.BaseData.type : PieceType.None;

            HexTileMap.RemovePiece(tile, startIcon.direction);
            HexTileMap.RemovePiece(tile, endIcon.direction);

            HexTileMap.AddPiece(tile, startIcon.direction, toStart);
            HexTileMap.AddPiece(tile, endIcon.direction, toEnd);
        }


        /// <summary>
        /// PieceIcon에 Piece 등록
        /// </summary>
        /// <param name="piece"></param>
        public void SetPiece(Piece piece)
        {
            if (!(selectedPiece = piece))
            {
                pieceImage.gameObject.SetActive(false);
                return;
            }
            pieceImage.gameObject.SetActive(pieceImage.sprite = selectedPiece.BaseData.pieceIcon);

            if (piece.DirectionPos != HexDirection.Center) return;
            pieceImage.transform.rotation = Quaternion.Euler(0, 0, -piece.LookDirection.ToDegree());
        }
    }
}