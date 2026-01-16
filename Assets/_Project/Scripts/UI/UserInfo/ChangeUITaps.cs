using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.UserInfo
{
    public static class ChangeUITabs
    {
        public static void ChangeTab(Button[] buttons, int index)
        {
            // 안눌린 버튼
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.OutCubic);
                buttons[i].image.color = new Color(0.6f, 0.6f, 0.6f);
            }

            // 눌린 버튼 하이라이트
            if (index >= 0 && index < buttons.Length)
            {
                buttons[index].transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f).SetEase(Ease.OutCubic);
                buttons[index].image.color = Color.white;
            }
        }

        public static void ChangeTab(Button clicked, bool isClicked)
        {
            if (isClicked)
            {
                clicked.transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f).SetEase(Ease.OutCubic);
                clicked.image.color = Color.white;
            }
            else
            {
                clicked.transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.OutCubic);
                clicked.image.color = new Color(0.6f, 0.6f, 0.6f);
            }
        }
    }
}
