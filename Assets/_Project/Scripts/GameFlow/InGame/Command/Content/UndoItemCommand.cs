using CocoDoogy.Data;
using CocoDoogy.Test;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class UndoItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;
        private ItemEffect Effect
        {
            get => ie;
            set => ie = value;
        }

        [SerializeField] private ItemEffect ie;

        public UndoItemCommand(object param) : base(CommandType.Undo)
        {
            Effect = (ItemEffect)param;
        }

        public override void Execute()
        {
            CommandManager.UndoCommandAuto();

            Debug.Log(DataManager.GetReplayItem(Effect));

            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), false);
            PlayerHandler.IsBehaviour = true;
        }

        public override void Undo()
        {
            CommandManager.RedoCommandAuto();
            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), true);
            PlayerHandler.IsBehaviour = false;
            
            TileOutlineDrawer.Draw();
        }
    }
}