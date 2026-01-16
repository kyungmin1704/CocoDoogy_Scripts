using UnityEngine;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Data;

namespace CocoDoogy.GameFlow.Replay
{
    public class ReplayHandler: MonoBehaviour
    {
        public static string ReplayData { get; set; }


        void Awake()
        {
            InGameManager.OnMapDrawn += OnMapDrawn;
        }
        void OnDestroy()
        {
            InGameManager.OnMapDrawn -= OnMapDrawn;
        }
        
        private void OnMapDrawn(StageData data)
        {
            CommandManager.Load(ReplayData);
        }
    }
}