using CocoDoogy.Audio;
using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class TeleportCommand : CommandBase
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


        public TeleportCommand(object param) : base(CommandType.Teleport)
        {
            var poses = ((Vector2Int, Vector2Int))param;
            PrePos = poses.Item1;
            NextPos = poses.Item2;
        }


        public override void Execute()
        {
            PlayerHandler.Tornado(NextPos);
            SfxManager.PlaySfx(SfxType.Weather_Wind);
        }

        public override void Undo()
        {
            PlayerHandler.Tornado(PrePos);
        }
    }
}