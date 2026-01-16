using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.UI.Popup
{
    public class TurnPagePanel : MonoBehaviour
    {
        private RectTransform rect = null;

        void OnEnable()
        {
            if (!rect) rect = GetComponent<RectTransform>();
            rect.localScale = new Vector3(0, 1, 1);
            rect.DOKill(true);
            rect.DOScale(new Vector2(1, 1), 0.5f).SetEase(Ease.OutCubic);
        }
    }
}