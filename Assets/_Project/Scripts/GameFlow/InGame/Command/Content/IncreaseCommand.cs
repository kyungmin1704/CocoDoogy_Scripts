using CocoDoogy.Audio;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class IncreaseCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [UnityEngine.SerializeField] private int rg = 0;


        public int Regen { get => rg; private set => rg = value; }
        
        
        public IncreaseCommand(object param) : base(CommandType.Increase)
        {
            Regen = (int)param;
        }

        
        public override void Execute()
        {
            HexTileMap.ActionPoint += Regen;
            InGameManager.RegenActionPoint(Regen, false);
        }

        public override void Undo()
        {
            HexTileMap.ActionPoint -= Regen;
            InGameManager.ConsumeActionPoint(Regen, false);
        }
    }
}