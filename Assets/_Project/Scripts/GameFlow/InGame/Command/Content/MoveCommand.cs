using CocoDoogy.Tile;
using UnityEngine;
using UnityEngine.Serialization;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class MoveCommand: CommandBase
    {
        public override bool IsUserCommand => true;


        [SerializeField] private HexDirection dir = HexDirection.East;
        
        
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public HexDirection Dir { get => dir; private set => dir = value; }


        public MoveCommand(object param): base(CommandType.Move)
        {
            Dir = (HexDirection)param;
        }


        public override void Execute()
        {
            InGameManager.ConsumeActionPoint(HexTile.GetTile(PlayerHandler.GridPos).CurrentData.RealMoveCost);
            
            PlayerHandler.LookDirection = Dir;
            Vector2Int nextPos = PlayerHandler.GridPos.GetDirectionPos(Dir);
            PlayerHandler.Move(nextPos);
            InGameManager.UseActionPoints += HexTile.GetTile(PlayerHandler.GridPos).CurrentData.RealMoveCost;
        }

        public override void Undo()
        {
            PlayerHandler.LookDirection = Dir;
            Vector2Int prePos = PlayerHandler.GridPos.GetDirectionPos(Dir.GetMirror());
            PlayerHandler.Move(prePos);
            InGameManager.UseActionPoints -= HexTile.GetTile(PlayerHandler.GridPos).CurrentData.RealMoveCost;
            InGameManager.RegenActionPoint(HexTile.GetTile(PlayerHandler.GridPos).CurrentData.RealMoveCost);
        }
    }
}