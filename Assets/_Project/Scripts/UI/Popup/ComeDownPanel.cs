using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.UI.Popup
{
    public class ComeDownPanel : MonoBehaviour
    {
        private RectTransform rect = null;


        void OnEnable()
        {
            if (!rect) rect = GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0, Screen.height);
            rect.DOKill(true);
            rect.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutCubic);
        }
    }
}