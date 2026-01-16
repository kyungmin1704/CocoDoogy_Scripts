using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CocoDoogy._Project.Scripts.UI.CocoDoogys
{
    public class GetItemPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panel;
        [SerializeField] private RectTransform target;
        [SerializeField] private TextMeshProUGUI amount;
        [SerializeField] private CocoDoogyUIIcon  cocoDoogyUIIcon;


        public static GetItemPanel instance;

        private void Awake()
        {
            instance = this;
            panel.gameObject.SetActive(false);
        }

        [ContextMenu("test")]
        public void test()
        {
            GetSomething(ItemUIType.Movement, 3,null);
        }

        public static void GetItem(ItemUIType itemUIType, int amount, Action callback = null)
        {
            if (!instance)
            {
                callback?.Invoke();
                return;
            }
            
            instance.GetSomething(itemUIType, amount, callback);
        }

        /// <summary>
        /// getSomething 이벤트로 호출되는 메서드
        /// ItemType을 매개변수로 받아 아이콘의 이미지를 변경 하고 amount로 갯수를 반영, callback에는 행동력이 오르는 메서드를 넣으면 됨
        /// </summary>
        /// <param name="itemUIType">어떤 아이템을 얻었는가</param>
        /// <param name="amount">얼마나 얻었는가</param>
        /// <param name="callback">실행 후에 무엇을 할 것인가</param>
        public void GetSomething(ItemUIType itemUIType, int amount,  Action callback)
        {
            MovePanelToPlayer();
            cocoDoogyUIIcon.ChangeIcon(itemUIType);
            this.amount.text = amount.ToString();
            panel.gameObject.SetActive(true);
            panel.DOLocalMove(panel.localPosition + new Vector3(0, 100), 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    panel.DOLocalMove(target.localPosition, 0.2f)
                        .SetEase(Ease.InOutCubic)
                        .OnComplete(() =>
                        {
                            panel.gameObject.SetActive(false);
                        }).SetUpdate(true);
                });
            callback?.Invoke();
        }
        private void MovePanelToPlayer()
        {
            Vector3 playerWorldPos = PlayerHandler.GridPos.ToWorldPos();
            // Canvas RectTransform 가져오기
            RectTransform canvasRect = panel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            // 월드 좌표 -> 스크린 좌표
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(playerWorldPos);
            // 스크린 좌표 -> Canvas 로컬 좌표
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPoint, 
                null, // Overlay Canvas일 땐 Camera null
                out localPoint
            );
            // Panel 위치 설정
            panel.localPosition = localPoint;
        }
        
    }
}