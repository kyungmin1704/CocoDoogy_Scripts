using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Command.Content;

namespace CocoDoogy.Replay
{
    public class ReplayPlayer : MonoBehaviour
    {
        public float delay = 0.5f; // 명령 간 딜레이 (초)
        private Stack<CommandBase> commandStack = new(); // 순차 실행용 스택

        public void StartReplay(string json)
        {
            CommandManager.Load(json);
            commandStack = new Stack<CommandBase>(CommandManager.Undid.Reverse()); // 역순으로 순차 실행
            StartCoroutine(PlayReplay());
        }

        private IEnumerator PlayReplay()
        {
            while (commandStack.Count > 0)
            {
                CommandBase command = commandStack.Pop();
                command.Execute(); // 명령 실행
                yield return new WaitForSeconds(delay); // 영상처럼 지연
            }

            Debug.Log("리플레이 종료");
        }
    }
}