using CocoDoogy.GameFlow.InGame;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.InGame
{
    /// <summary>
    /// InGame 내에서 래버, 버튼 등의 트리거를 동작시키기 위한 상호작용 버튼 핸들러
    /// </summary>
    public class InteractButton: MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private CommonButton button;


        private Action action = null;


        void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
            OnInteractChanged(null, false, null);

            InGameManager.OnInteractChanged += OnInteractChanged;
        }
        void OnDestroy()
        {
            InGameManager.OnInteractChanged -= OnInteractChanged;
        }


        private void OnButtonClicked()
        {
            action?.Invoke();
        }


        private void OnInteractChanged(Sprite icon, bool interactable, Action callback)
        {
            iconImage.gameObject.SetActive(iconImage.sprite = icon);
            button.interactable = interactable;
            action = callback;
        }

        public void OnInteractChanged()
        {
            button.interactable = false;
        }
    }
}
