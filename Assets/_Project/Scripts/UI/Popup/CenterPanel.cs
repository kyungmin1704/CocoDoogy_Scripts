using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.UI.Popup
{
    public class CenterPanel : MonoBehaviour
    {
        private RectTransform rect = null;
        
        void OnEnable()
        {
            if (!rect) rect = GetComponent<RectTransform>();

            rect.localScale = Vector3.zero;
            rect.DOKill(true);
            rect.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }
}
