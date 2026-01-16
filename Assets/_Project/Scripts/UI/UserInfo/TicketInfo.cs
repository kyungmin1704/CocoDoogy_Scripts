using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.UserInfo
{
    public class TicketInfo : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameObject realTimeTicket;
        
       
        private void Awake()
        {
            if (realTimeTicket != null)
                realTimeTicket.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (realTimeTicket == null)
                return;

            bool willOpen = !realTimeTicket.activeSelf;

            realTimeTicket.SetActive(willOpen);

            if (willOpen)
            {
                // 팝업을 "현재 선택된 UI"로 지정
                var target = realTimeTicket;

                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(target);
            }
            else
            {
                // 닫힐 때는 선택도 비워줌 (선택 관련 꼬임 방지용)
                if (EventSystem.current != null &&
                    EventSystem.current.currentSelectedGameObject == realTimeTicket)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }
    }
}
