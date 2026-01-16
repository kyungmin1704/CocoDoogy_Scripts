using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using CocoDoogy.UI;
using Lean.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MapEditor.GimmickConnector
{
    public class GimmickTriggerButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CommonButton deleteButton;
        [SerializeField] private Toggle reverseToggle;


        private GimmickData targetParent = null;
        private GimmickTrigger target = null;


        void Reset()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            deleteButton = GetComponentInChildren<CommonButton>();
            reverseToggle = GetComponentInChildren<Toggle>();
        }

        void Awake()
        {
            deleteButton.onClick.AddListener(OnClick);
            reverseToggle.onValueChanged.AddListener(OnToggleChanged);
        }


        /// <summary>
        /// Data를 받아서, UI 설정 및 Target 지정
        /// </summary>
        /// <param name="gimmick"></param>
        /// <param name="triggerData"></param>
        public void Init(GimmickData gimmick, GimmickTrigger triggerData)
        {
            targetParent = gimmick;
            target = triggerData;

            HexTile tile = HexTile.GetTile(target.GridPos);
            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            text.text = $"{tile.CurrentData.type} {target.GridPos}({(centerPiece ? centerPiece.BaseData.type : "<color=red><b>ERROR</b></color>")})";
            reverseToggle.SetIsOnWithoutNotify(target.IsReversed);
        }


        private void OnClick()
        {
            targetParent.Triggers.Remove(target);

            LeanPool.Despawn(this);
        }
        private void OnToggleChanged(bool isOn)
        {
            target.IsReversed = isOn;
        }
    }
}