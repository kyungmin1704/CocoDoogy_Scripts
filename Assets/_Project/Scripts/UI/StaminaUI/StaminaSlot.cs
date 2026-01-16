using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*  
    ActionPointSlot은 행동력의 각 칸을 나타내는 UI 요소입니다.
    활성/비활성 상태를 애니메이션으로 표현합니다.
*/

namespace CocoDoogy.UI.StaminaUI
{
    public class StaminaSlot : MonoBehaviour
    {
        [Header("UI 참조")]
        [SerializeField] private Image slotImage;
        [SerializeField] private Image fillImage;

        [Header("색상 설정")]
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        [Header("애니메이션 설정")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private float scaleMultiplier = 1.2f;
        [SerializeField] private Ease animationEase = Ease.OutBack;

        private bool isActive = false;
        private Sequence currentSequence;

        private void Awake()
        {
            if (slotImage == null)
                slotImage = GetComponent<Image>();

            if (fillImage == null && transform.childCount > 0)
                fillImage = transform.GetChild(0).GetComponent<Image>();
        }

        // 슬롯 활성화
        public void Activate()
        {
            if (isActive) return;

            isActive = true;

            PlayActivateAnimation();
        }

        // 슬롯 비활성화
        public void Deactivate()
        {
            if (!isActive) return;

            isActive = false;

            PlayDeactivateAnimation();
        }

        private void PlayActivateAnimation()
        {
            KillCurrentAnimation();

            currentSequence = DOTween.Sequence();

            // 스케일 애니메이션
            currentSequence.Append(transform.DOScale(scaleMultiplier, animationDuration * 0.5f)
                .SetEase(animationEase));
            currentSequence.Append(transform.DOScale(1f, animationDuration * 0.5f)
                .SetEase(Ease.OutQuad));

            // 색상 애니메이션
            if (fillImage != null)
            {
                currentSequence.Join(fillImage.DOColor(activeColor, animationDuration));
            }

            if (slotImage != null)
            {
                currentSequence.Join(slotImage.DOColor(activeColor, animationDuration));
            }
        }

        private void PlayDeactivateAnimation()
        {
            KillCurrentAnimation();

            currentSequence = DOTween.Sequence();

            // 페이드아웃 애니메이션
            if (fillImage != null)
            {
                currentSequence.Append(fillImage.DOColor(inactiveColor, animationDuration)
                    .SetEase(Ease.InQuad));
            }

            if (slotImage != null)
            {
                currentSequence.Join(slotImage.DOColor(inactiveColor, animationDuration)
                    .SetEase(Ease.InQuad));
            }
        }

        private void SetInactiveImmediate()
        {
            KillCurrentAnimation();

            transform.localScale = Vector3.one;

            if (fillImage != null)
                fillImage.color = inactiveColor;

            if (slotImage != null)
                slotImage.color = inactiveColor;
        }

        private void KillCurrentAnimation()
        {
            if (currentSequence != null && currentSequence.IsActive())
            {
                currentSequence.Kill();
                currentSequence = null;
            }
        }

        private void OnDestroy()
        {
            KillCurrentAnimation();
        }
    }
}

