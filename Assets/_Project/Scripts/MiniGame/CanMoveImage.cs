using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Outline))]
    public class CanMoveImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        protected Image image;
        protected Outline outline;

        protected Vector2 orginalOutliner = new Vector2(0,0);
        protected virtual void Awake()
        {
            image = GetComponent<Image>();
            outline = GetComponent<Outline>();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            outline.effectDistance = new Vector2(10f, 10f);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            outline.effectDistance = orginalOutliner;
        }

        /// <summary>
        /// 이미지가 따라오도록함
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            
            transform.position = eventData.position;
        }
        
        /// <summary>
        /// 드래그가 끝났을시 오브젝트의 위치를 그대로 둠
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
        }
        
        
        

        public virtual void OnBeginDrag(PointerEventData eventData)
        {

        }
    }
}
