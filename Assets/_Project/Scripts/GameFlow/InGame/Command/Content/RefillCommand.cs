using CocoDoogy.Audio;
using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 행동력 부족 및 이동 불가능 등으로 인한 초기화 동작
    /// </summary>
    [System.Serializable]
    public class RefillCommand: CommandBase
    {
        public override bool IsUserCommand => true;


        [SerializeField] private int rp = 0;
        [SerializeField] private Vector2Int gp = Vector2Int.zero;
        [SerializeField] private int sc = 0;


        /// <summary>
        /// 남은 포인트
        /// </summary>
        public int RemainPoints { get => rp; private set => rp = value; }
        /// <summary>
        /// 마지막 위치
        /// </summary>
        public Vector2Int GridPos { get => gp; private set => gp = value; }
        /// <summary>
        /// 
        /// </summary>
        public int SandCount { get => sc; private set => sc = value; }
        
        
        public RefillCommand(object param) : base(CommandType.Refill)
        {
            var data = ((int, Vector2Int, int))param;
            RemainPoints = data.Item1;
            GridPos = data.Item2;
            SandCount = data.Item3;
        }

        
        public override void Execute()
        {
            InGameManager.ConsumeActionPoint(RemainPoints);
            InGameManager.RefillActionPoint();
            PlayerHandler.SandCount = 0;
            SfxManager.PlaySfx(SfxType.UI_Reset);
            PlayerHandler.Deploy(HexTileMap.StartPos);
        }

        public override void Undo()
        {
            InGameManager.ClearActionPoint();
            InGameManager.RegenActionPoint(RemainPoints);
            PlayerHandler.SandCount = SandCount;
            
            PlayerHandler.Deploy(GridPos);
        }
    }
}