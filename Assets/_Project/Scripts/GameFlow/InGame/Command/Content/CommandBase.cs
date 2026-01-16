using System;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public abstract class CommandBase
    {
        [NonSerialized] public CommandType Type;

        public abstract bool IsUserCommand { get; }


        public CommandBase(CommandType type)
        {
            Type = type;
        }


        /// <summary>   
        /// 동작
        /// </summary>
        public abstract void Execute();
        /// <summary>
        /// 되돌리기
        /// </summary>
        public abstract void Undo();
    }
}