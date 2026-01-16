using Coffee.UIExtensions;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private Sprite brightStarSprite;
        [SerializeField] private Sprite darkStarSprite;
        [SerializeField] private UIParticle starParticle;
        
        private Image image;
        private RectTransform rectTransform;
        private Vector2 originalScale;
        
        private float scaleUp = 1.3f;
        private float scaleDown = 0.7f;
        private float duration = 0.125f;
        

        private void Awake()
        {
            image =  GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            originalScale = rectTransform.localScale;
        }


         /// <summary>
         /// DOTween으로 크기 변경
         /// </summary>
         [ContextMenu("TryGetStar실행")]
         public void TryGetStar(Action callback)
         {
             PlayStarParticle();
             rectTransform.DOKill();
             image.sprite = brightStarSprite;
             Sequence seq = DOTween.Sequence();
             seq.Append(rectTransform.DOScale(scaleUp, duration).SetEase(Ease.OutQuad))
                 .Append(rectTransform.DOScale(scaleDown, duration).SetEase(Ease.OutQuad))
                 .Append(rectTransform.DOScale(originalScale, duration))
                 .OnComplete(() =>
                 {
                     callback?.Invoke(); 
                 });
         }

         public void PlayStarParticle()
         {
             starParticle.Play();
         }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            image.sprite = darkStarSprite;
            rectTransform.DOKill();
        }

        private void OnDisable()
        {
            Initialize();
        }
    }
}