using CocoDoogy.Audio;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.UI.Popup;
using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    /// <summary>
    /// LeverType용 트리거<br/>
    /// 토글 형태로 동작
    /// </summary>
    public class LeverPiece : TriggerPieceBase, IInteractable
    {
        [SerializeField] private Transform lever;


        public bool CanInteract => HexTileMap.GetTriggers(Parent.GridPos).Length > 0;
        public Sprite Icon => DataManager.GetPieceData(PieceType.Lever).pieceIcon;

        public override bool IsOn
        {
            get => isOn;
            set
            {
                lever.localRotation = Quaternion.Euler(0, (isOn = value) ? 180 : 0, 0);
            }
        }


        private bool isOn = false;


        public override void OnRelease(Piece data)
        {
            IsOn = false;
        }
        

        public override void Interact()
        {
            if (IsOn)
            {
                SfxManager.PlaySfx(SfxType.Interaction_LeverOff);
            }
            else
            {
                SfxManager.PlaySfx(SfxType.Interaction_LeverOn);
            }

            
            IsOn = !IsOn;
        }
        public override void UnInteract() => Interact();


        public void OnInteractClicked()
        {
            MessageDialog.ShowMessage("기믹 동작", "해당 타일에 있는 래버를 당길거야?", DialogMode.YesNo, OnTriggerControlled);
        }

        private void OnTriggerControlled(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                CommandManager.Trigger(Parent.GridPos);
                GimmickExecutor.ExecuteFromTrigger(Parent.GridPos);
                InGameManager.ProcessPhase();
            }
        }
    }
}