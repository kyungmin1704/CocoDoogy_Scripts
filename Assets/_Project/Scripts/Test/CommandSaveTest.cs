using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CocoDoogy.Test
{
    public class CommandSaveTest: MonoBehaviour
    {
        public static string ReplayData { get; set; }
        
        [ContextMenu("Load")]
        private void Load()
        {
            CommandManager.Load(ReplayData);
        }
    }
}