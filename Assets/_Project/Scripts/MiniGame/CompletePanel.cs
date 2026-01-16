using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    public class CompletePanel : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image completeImage;
        [SerializeField] private TextMeshProUGUI clearText;
        
        private Action clickAction;

        /// <summary>
        /// Complete Image를 보여주는 함수
        /// </summary>
        /// <param name="onClicked"></param>
        public void Show(Action onClicked)
        {
            clickAction = onClicked;
            gameObject.SetActive(true);

            // 초기 스케일 세팅
            completeImage.rectTransform.localScale = Vector3.zero;
            clearText.rectTransform.localScale = Vector3.zero;

            // 등장 애니메이션 (1회)
            completeImage.rectTransform
                .DOScale(Vector3.one, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // completeImage 무한 반복 scale 효과
                    completeImage.rectTransform
                        .DOScale(1.1f, 0.8f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });

            // 텍스트도 등장 후 반복
            clearText.rectTransform
                .DOScale(Vector3.one, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // clearText 무한 반복 scale 효과
                    clearText.rectTransform
                        .DOScale(1.1f, 0.8f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });
        }

        /// <summary>
        /// 터치 입력감지
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            clickAction?.Invoke();
            clickAction = null;
        }
        

        /// <summary>
        /// 숨기기
        /// </summary>
        public void Hide()
        {
            // DOTween 적용한 것들 모두 제거
            completeImage.rectTransform.DOKill();
            clearText.rectTransform.DOKill();
            
            gameObject.SetActive(false);
        }
    }
}
