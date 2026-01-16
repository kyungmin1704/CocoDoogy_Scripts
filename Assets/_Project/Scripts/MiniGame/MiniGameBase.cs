using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    public abstract class MiniGameBase : MonoBehaviour
    {
        [Tooltip("해당 미니게임 등장 가능 계절 테마")]
        [SerializeField] private Theme themeFlag;
        [SerializeField] protected Image background;
        [SerializeField] protected TextMeshProUGUI remainCount;
        
        [Tooltip("미니게임 설명 여부 체크용")]
        [SerializeField] private string miniGameID;
        [SerializeField] private CompletePanel  completePanel;

        public TutorialExplainData tutorialExplainData;
        public string  MiniGameID => miniGameID;
        protected Action remainCountCallback;
        protected Action clearCallback;

        private bool hasStarted = false;
        /// <summary>
        /// 해당 테마에 사용 가능한 미니게임 여부
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public bool HasWithTheme(Theme theme) => themeFlag.HasFlag(theme);

        /// <summary>
        /// 미니게임 시작
        /// </summary>
        /// <param name="callback"></param>
        public void Open(Action callback)
        {
            hasStarted = true;
            
            clearCallback = callback;
            gameObject.SetActive(true);
            OnOpenInit();
            ShowRemainCount();
            remainCountCallback = ShowRemainCount;
            SfxManager.PlaySfx(SfxType.Gimmick_HouseEnter);
            SfxManager.PlaySfx(SfxType.Minigame_MinigameStart);
        }


        /// <summary>
        /// 미니게임 클리어 판단
        /// </summary>
        public void CheckClear()
        {
            remainCountCallback?.Invoke();
            if (!IsClear()) return;
            if (!hasStarted) return;
            hasStarted = false;
            SfxManager.PlaySfx(SfxType.UI_SuccessMission);
            completePanel.Show(() =>
            {
                Disable();
                gameObject.SetActive(false);
                MiniGameManager.Instance.BackGround.SetActive(false);
                MiniGameManager.Instance.LetterBoxd.SetActive(false);
                clearCallback?.Invoke();
                completePanel.Hide();
            });
        }
        /// <summary>
        /// 클리어까지 남은 진행도를 표시
        /// </summary>
        protected abstract void ShowRemainCount();
        /// <summary>
        /// 미니게임 배경 세팅
        /// </summary>
        /// <param name="sprite"></param>
        protected abstract void SetBackground(Sprite sprite);
        /// <summary>
        /// 미니게임 시작 세팅
        /// </summary>
        protected abstract void OnOpenInit();
        /// <summary>
        /// 미니게임 클리어 조건
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsClear();
        /// <summary>
        /// 미니게임 초기화
        /// </summary>
        protected abstract void Disable();

        protected abstract void ResizePanel();
    }
}
