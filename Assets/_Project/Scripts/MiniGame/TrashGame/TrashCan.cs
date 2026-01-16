using CocoDoogy.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.MiniGame.TrashGame
{
    public class TrashCan : MonoBehaviour, IPointerClickHandler
    {
        RectTransform rect;
        private Tween shakeTween;
        /// <summary>
        /// TODO: 사운드 필요
        /// </summary>
        /// <param name="eventData"></param>
        /// 
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 시각적 효과
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            SfxManager.PlaySfx(SfxType.Minigame_TouchTrashCan);
            if (shakeTween != null && shakeTween.IsActive()) shakeTween.Kill();
            shakeTween = rect.DOShakeAnchorPos(duration:0.3f, strength: 30f, vibrato: 20, randomness:90f,snapping: false,fadeOut: true);

        }

        /// <summary>
        /// 시각적 효과
        /// </summary>
        public void ShakingWithTilt()
        {
            if (shakeTween != null && shakeTween.IsActive()) shakeTween.Kill();

            shakeTween = DOTween.Sequence()
                .Append(rect.DOShakeAnchorPos(0.3f, new Vector2(10, 0), vibrato: 10, randomness: 0, fadeOut: true))
                
                .Join(rect.DOPunchRotation( new Vector3(0, 0, 5f), 0.3f, vibrato: 10,elasticity:1));

        }

    }
}
