namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class SandCountCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [UnityEngine.SerializeField] private int pc = 0;
        [UnityEngine.SerializeField] private int nc = 0;
        
        
        /// <summary>
        /// 이전 모래 카운터
        /// </summary>
        public int PreCount { get => pc; private set => pc = value; }
        /// <summary>
        /// 이후 모래 카운터
        /// </summary>
        public int NextCount { get => nc; private set => nc = value; }


        public SandCountCommand(object param): base(CommandType.SandCount)
        {
            var data = ((int, int))param;
            PreCount = data.Item1;
            NextCount = data.Item2;
        }


        public override void Execute()
        {
            PlayerHandler.SandCount = NextCount;
        }

        public override void Undo()
        {
            PlayerHandler.SandCount = PreCount;
        }
    }
}