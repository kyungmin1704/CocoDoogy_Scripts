using System;

namespace CocoDoogy.GameFlow.InGame.Phase.Passage
{
    public abstract class PassageBase
    {
        public int ActionPoints { get; protected set; }
        
        
        public abstract void Execute();
    }
}