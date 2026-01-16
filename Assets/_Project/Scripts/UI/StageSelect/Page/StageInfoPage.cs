using CocoDoogy.Data;
using System;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Page
{
    public abstract class StageInfoPage : MonoBehaviour
    {
        public StageData StageData { get; private set; } = null;


        private RectTransform rect;


        public void Show(StageData data)
        {
            if (!rect)
            {
                rect = GetComponent<RectTransform>();
            }

            StageData = data;
            gameObject.SetActive(true);
            OnShowPage();
        }
        public void Close(Action onTurnComplete = null)
        {
            WindowAnimation.TurnPage(rect, onTurnComplete);
        }

        protected abstract void OnShowPage();
    }
}