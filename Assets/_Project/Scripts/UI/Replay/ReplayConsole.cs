using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Command.Content;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Replay
{
    public class ReplayConsole : MonoBehaviour
    {
        [SerializeField] private Button undoButton; 
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button redoButton;
        
        [SerializeField] private float delay = 0.5f; // 명령 간 딜레이 (초)

        [SerializeField] private GameObject playIcon;
        [SerializeField] private GameObject pauseIcon;
        
        private bool IsPaused;
        private void Awake()
        {
            undoButton.interactable = false;
            pauseButton.interactable = true;
            redoButton.interactable = false;
            
            redoButton.onClick.AddListener(Redo);
            undoButton.onClick.AddListener(Undo);
            pauseButton.onClick.AddListener(ReplayPause);
            
            playIcon.gameObject.SetActive(IsPaused);
            pauseIcon.gameObject.SetActive(!IsPaused);
        }


        private void Start()
        {
            StartReplay().Forget();
        }

        private void ReplayPause()
        {
            IsPaused = !IsPaused;
            undoButton.interactable = !undoButton.interactable;
            redoButton.interactable = !redoButton.interactable;
            playIcon.gameObject.SetActive(IsPaused);
            pauseIcon.gameObject.SetActive(!IsPaused);
        }
        
        private async UniTaskVoid StartReplay()
        {
            await UniTask.WaitUntil(() => CommandManager.Undid.Count > 0);

            await PlayReplay();
        }

        private async UniTask PlayReplay()
        {
            while (CommandManager.Undid.Count > 0)
            {
                // 일시정지 처리
                await UniTask.WaitWhile(() => IsPaused);

                CommandManager.RedoCommandAuto();

                await UniTask.Delay((int)(delay * 1000)); // ms 단위
            }
        }

        private void Undo()
        {
            undoButton.interactable = false;
            redoButton.interactable = false;
            pauseButton.interactable = false;
            CommandManager.UndoCommandAuto();
            _ = StateChange();
        }

        private void Redo()
        {
            undoButton.interactable = false;
            redoButton.interactable = false;
            pauseButton.interactable = false;
            CommandManager.RedoCommandAuto();
            _ = StateChange();
        }

        private async UniTask StateChange()
        {
            await UniTask.Delay((int)(delay * 1000));
            undoButton.interactable = true;
            redoButton.interactable = true;
            pauseButton.interactable = true;
        }
        
    }
}