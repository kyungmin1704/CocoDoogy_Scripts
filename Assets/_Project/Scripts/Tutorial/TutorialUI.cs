using CocoDoogy.Core;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.Tutorial
{
    /// <summary>
    /// 튜토리얼 UI 컨트롤러
    /// 클릭 시 다음 메시지로 넘어가고, 처음만 표시하는 로직 포함
    /// PlayerPrefs로 로컬에서만 관리
    /// </summary>
    public class TutorialUI : Singleton<TutorialUI>, IPointerClickHandler
    {
        public static event Action OnTouched = null;


        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image messageBox;
        [SerializeField] private Image backgroundImage;


        private bool preRaycastTarget = true;
        private float lastTouchTime = 0f;


        protected override void Awake()
        {
            base.Awake();
            lastTouchTime = Time.time;
        }
        

        public static void Show(string content)
        {
            if(!Instance) return;
            
            if(Instance.preRaycastTarget)
            {
                OnRaycast();
            }

            Instance.descriptionText.text = content;

            Instance.messageBox.gameObject.SetActive(true);
            Instance.backgroundImage.gameObject.SetActive(true);
        }
        public static void Close()
        {
            if(!Instance) return;

            Instance.preRaycastTarget = Instance.messageBox.raycastTarget;
            OffRaycast();
            Instance.messageBox.transform.DOScale(0f, 0.2f).SetId(Instance).OnComplete(Instance.OnCloseComplete);
        }

        public static void OnRaycast()
        {
            if(!Instance) return;

            Instance.descriptionText.raycastTarget =
                Instance.messageBox.raycastTarget =
                Instance.backgroundImage.raycastTarget = true;
        }
        public static void OffRaycast()
        {
            if(!Instance) return;

            Instance.descriptionText.raycastTarget =
                Instance.messageBox.raycastTarget =
                Instance.backgroundImage.raycastTarget = false;
        }

        public static void OnBackground()
        {
            if(!Instance) return;

            Instance.backgroundImage.color = new Color(0, 0, 0, 200f / 255f);
        }
        public static void OffBackground()
        {
            Instance.backgroundImage.color = Color.clear;
        }

        private void OnCloseComplete()
        {
            messageBox.gameObject.SetActive(false);
            backgroundImage.gameObject.SetActive(false);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(lastTouchTime + 0.1f > Time.time) return;

            lastTouchTime = Time.time;
            OnTouched?.Invoke();
        }
    }
}

