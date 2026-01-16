using CocoDoogy.Data;
using CocoDoogy.Tile.Piece;
using CocoDoogy.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MapEditor.UI
{
    /// <summary>
    /// 버튼을 선택할 때 건물의 인덱스를 기준으로 건물을 선택하기 위해서 만든 클래스
    /// </summary>
    public class PieceButton : MonoBehaviour
    {
        [Tooltip("기물 아이콘")][SerializeField] private Image pieceImage;
        [Tooltip("기물 명칭")][SerializeField] private TextMeshProUGUI nameText;

        [Tooltip("클릭 이벤트 호출용 버튼")][SerializeField] private CommonButton button;


        private PieceData pieceData = null;
        private Action<PieceType> callback = null;


        void Reset()
        {
            pieceImage = GetComponentInChildren<Image>();
            nameText = GetComponentInChildren<TextMeshProUGUI>();

            button = GetComponentInChildren<CommonButton>();
        }


        // 버튼 UI에 정보 입력 및 클릭 콜백 지정
        public void Init(PieceType pieceType, Action<PieceType> callback)
        {
            pieceData = DataManager.GetPieceData(pieceType);
            if (pieceData == null)
            {
                Debug.LogError("Tile Data Not Found.");
                return;
            }

            this.callback = callback;
            pieceImage.sprite = pieceData.pieceIcon;
            nameText?.SetText(pieceData.pieceName);
            button.onClick.AddListener(OnButtonClicked);
        }


        // 해당 기물 버튼 클릭 시
        private void OnButtonClicked()
        {
            callback?.Invoke(pieceData.type);
        }
    }
}