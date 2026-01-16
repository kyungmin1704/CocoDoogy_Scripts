using CocoDoogy.Animation;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.LifeCycle;
using CocoDoogy.UI.Popup;
using System;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    [RequireComponent(typeof(Piece))]
    public class DeckPiece: MonoBehaviour, IInit<Piece>,  ISpecialPiece, IInteractable
    {
        [SerializeField] private GameObject boatObject;


        public bool CanInteract => piece.Target.HasValue && InGameManager.ActionPoints > 0;
        public Sprite Icon => DataManager.GetPieceData(PieceType.Deck).pieceIcon;

        public bool IsDocked
        {
            get => isDocked;
            set => boatObject.SetActive(isDocked = value);
        }
        public bool PreDocked => preDocked;


        private bool isDocked = false;
        private bool preDocked = false;

        private Piece piece = null;


        public void OnInit(Piece piece)
        {
            this.piece = piece;
        }


        public void OnDataInsert(string data)
        {
            if (!bool.TryParse(data, out bool docked)) docked = false;
            boatObject.SetActive(IsDocked = preDocked = docked);
        }

        public void OnExecute()
        {
            boatObject.SetActive(IsDocked = !IsDocked);
        }


        public void OnInteractClicked()
        {
            MessageDialog.ShowMessage("승선 확인", "배에 올라탈 거야?", DialogMode.YesNo, OnTriggerControlled);
        }
        private void OnTriggerControlled(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                CommandManager.Sail(piece.Target.Value);
                VehicleAnimHandler.SetActive();
            }
        }
    }
}