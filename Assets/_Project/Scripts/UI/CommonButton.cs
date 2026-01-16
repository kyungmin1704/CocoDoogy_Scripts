using CocoDoogy.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.UI
{
    /// <summary>
    /// Tween 효과를 주는 Button Component
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class CommonButton : Button
    {
        private RectTransform rect;
        private Color buttonColor;


#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            interactable = true;
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            rect = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            buttonColor = image.color;
        }


        #region ◇ Tween효과 ◇
        public override void OnPointerDown(PointerEventData data)
        {
            if (!interactable) return;

            DOTween.Kill(this);

            rect.DOScale(0.9f, 0.15f).SetEase(Ease.OutCubic).SetId(this);
            image.DOColor(buttonColor * 0.8f, 0.15f).SetEase(Ease.OutCubic).SetId(this);
            SfxManager.PlaySfx(SfxType.UI_ButtonDown);
        }

        public override void OnPointerUp(PointerEventData data)
        {
            if (!interactable) return;

            DOTween.Kill(this);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOScale(new Vector2(1.1f, 0.9f), 0.1f).SetEase(Ease.OutCubic));
            sequence.Append(rect.DOScale(new Vector2(0.9f, 1.1f), 0.1f).SetEase(Ease.OutCubic));
            sequence.Append(rect.DOScale(Vector2.one, 0.1f).SetEase(Ease.OutCubic));
            sequence.SetId(this);
            sequence.Play();
            SfxManager.PlaySfx(SfxType.UI_ButtonUp1);
            image.DOColor(buttonColor, 0.2f).SetEase(Ease.OutBack).SetId(this);
        }
        #endregion
    }
}