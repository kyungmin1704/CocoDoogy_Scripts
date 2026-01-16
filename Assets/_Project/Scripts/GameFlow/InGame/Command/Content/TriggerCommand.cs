using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Tile.Piece.Trigger;
using UnityEngine;
using UnityEngine.Serialization;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class TriggerCommand: CommandBase
    {
        public override bool IsUserCommand => true;


        [SerializeField] private Vector2Int gp = Vector2Int.zero;
        [SerializeField] private byte un = 0;
        
        
        /// <summary>
        /// 트리거 위치
        /// </summary>
        public Vector2Int GridPos { get => gp; private set => gp = value; }
        public bool IsUnInteract { get => un == 1; private set => un = value ? (byte)1 : (byte)0; }


        public TriggerCommand(object param): base(CommandType.Trigger)
        {
            var data = ((Vector2Int, bool))param;
            GridPos = data.Item1;
            IsUnInteract = data.Item2;
        }


        private TriggerPieceBase TriggerPiece
        {
            get
            {
                HexTile tile = HexTile.GetTile(GridPos);
                if (!tile) return null;

                Piece piece = tile.GetPiece(HexDirection.Center);
                if (!piece) return null;

                TriggerPieceBase result = piece.GetComponent<TriggerPieceBase>();

                return result;
            }
        }


        public override void Execute()
        {
            TriggerPieceBase trigger = TriggerPiece;
            if (!trigger) return;

            if (!IsUnInteract)
            {
                trigger.Interact();
            }
            else
            {
                trigger.UnInteract();
            }
        }

        public override void Undo()
        {
            TriggerPieceBase trigger = TriggerPiece;
            if (!trigger) return;

            if (!IsUnInteract)
            {
                trigger.UnInteract();
            }
            else
            {
                trigger.Interact();
            }
        }
    }
}