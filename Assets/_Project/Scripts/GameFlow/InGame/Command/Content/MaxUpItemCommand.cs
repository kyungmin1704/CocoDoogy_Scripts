using CocoDoogy.Data;
using CocoDoogy.Test;
using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class MaxUpItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;
        private ItemEffect Effect
        {
            get => itemEffect;
            set => itemEffect = value;
        }
        
        [SerializeField] private ItemEffect itemEffect;
        
        private const int Delta = 1;
        
        public MaxUpItemCommand(object param) : base(CommandType.MaxUp)
        {
            Effect = (ItemEffect)param;
        }
        
        public override void Execute()
        {
            HexTileMap.ActionPoint += Delta;
            InGameManager.ConsumeActionPoint(Delta, true);
            
            InGameManager.UseActionPoints++;
            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), false);
            PlayerHandler.IsBehaviour = true;
            
            TileOutlineDrawer.Draw();
        }

        public override void Undo()
        {
            HexTileMap.ActionPoint -= Delta;
            InGameManager.RegenActionPoint(Delta, true);
            InGameManager.UseActionPoints--;
            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), true);
            PlayerHandler.IsBehaviour = false;
        }
    }
}