using CocoDoogy.GameFlow.InGame.Command;
using UnityEngine;

namespace CocoDoogy.Test
{
    public class InGameTestButtons: MonoBehaviour
    {
        public void Undo()
        {
            CommandManager.UndoCommandAuto();
        }
        public void Redo()
        {
            CommandManager.RedoCommandAuto();
        }
    }
}