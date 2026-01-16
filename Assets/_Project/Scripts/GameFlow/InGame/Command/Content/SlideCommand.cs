using CocoDoogy.Audio;
using CocoDoogy.Tile;
using UnityEngine;
using UnityEngine.Serialization;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class SlideCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [UnityEngine.SerializeField] private Vector2Int pp = Vector2Int.zero;
        [UnityEngine.SerializeField] private Vector2Int np = Vector2Int.zero;
        
        
        /// <summary>
        /// 이전 위치
        /// </summary>
        public Vector2Int PrePos { get => pp; private set => pp = value; }

        /// <summary>
        /// 다음 위치
        /// </summary>
        public Vector2Int NextPos { get => np; private set => np = value; }


        public SlideCommand(object param): base(CommandType.Slide)
        {
            var data = ((Vector2Int, Vector2Int))param;
            PrePos = data.Item1;
            NextPos = data.Item2;
        }


        public override void Execute()
        {
            SfxManager.PlaySfx(SfxType.Interaction_Sliding);
            PlayerHandler.Slide(NextPos);
        }

        public override void Undo()
        {
            PlayerHandler.Slide(PrePos);
        }
    }
}