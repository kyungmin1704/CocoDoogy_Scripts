using CocoDoogy.Network;
using CocoDoogy.UI.UIManager;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CocoDoogy.UI.IntroAndLogin
{
    public class IntroUI : UIPanel
    {
        [Header("UI Elements")]
        [SerializeField] private Image titleImage;
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI touchToStartText;

        [Header("Shake Title Options")]
        [SerializeField] private float moveAmount = 20f;
        [SerializeField] private float titleDuration = 0.5f;
        [SerializeField] private float textDuration = 1f;
        [SerializeField] private float interval = 1f;
        private RectTransform rect;

        [Header("Blick Text Effect")]
        [Range(0, 0.3f)]
        [SerializeField] private float minBlinkRatio;
        
        private void Awake()
        {
            startButton.interactable = false;
            startButton.onClick.AddListener(ConvertLoginUI);
            ShakeTitleImage();
            BlickText();
        }
        
        /// <summary>
        /// 맨 처음 화면을 클릭하면 TitleImage를 한번 위 아래로 흔들며 강조하는 메서드
        /// </summary>
        private void ShakeTitleImage()
        {
            rect = titleImage.GetComponent<RectTransform>();
            Vector2 originalPos = rect.anchoredPosition;

            Sequence seq = DOTween.Sequence();

            seq.Append(rect.DOAnchorPosY(originalPos.y + moveAmount, titleDuration))
                .Append(rect.DOAnchorPosY(originalPos.y - moveAmount, titleDuration))
                .Append(rect.DOAnchorPosY(originalPos.y, titleDuration)).OnComplete(() => startButton.interactable = true);
        }

        /// <summary>
        /// touchToStartText를 깜빡이게 하는 메서드
        /// </summary>
        private void BlickText()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(touchToStartText.DOFade(minBlinkRatio, textDuration))
                .Append(touchToStartText.DOFade(1, textDuration)).SetLoops(-1);
        }

        /// <summary>
        /// Intro에서 LoginUI로 변경하는 메서드 <br/>
        /// 맨 처음 화면에서 로그인 버튼이 있는 화면으로 이동
        /// </summary>
        private void ConvertLoginUI()
        {
            startButton.interactable = false;
            IntroUIManager.Instance.LoginUI.OpenPanel();
            touchToStartText.gameObject.SetActive(false);
        }

        public override void ClosePanel()
        {
            // Intro는 Lobby로 넘어가기 전까지 계속 나와야 하므로 Close 기능 없음 (기능 X)
        }
    }
}
