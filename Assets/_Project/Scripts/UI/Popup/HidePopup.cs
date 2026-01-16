using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.Popup
{
    public class HidePopup : MonoBehaviour, IDeselectHandler
    {
        [SerializeField] private GameObject popup;

        public void OnDeselect(BaseEventData eventData)
        {
            if (popup != null)
                popup.SetActive(false);
        }
    }
}
