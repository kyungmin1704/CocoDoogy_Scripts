using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile.Gimmick;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase.Passage
{
    public class ButtonPassage: PassageBase
    {
        public Vector2Int GridPos => gridPos;
        
        
        private readonly Vector2Int gridPos = Vector2Int.zero;
        
        
        public ButtonPassage(int actionPoints, Vector2Int gridPos)
        {
            ActionPoints = actionPoints;
            this.gridPos = gridPos;
        }
        
        
        public override void Execute()
        {
            CommandManager.Trigger(GridPos, true);
            GimmickExecutor.ExecuteFromTrigger(GridPos);
        }
    }
}