using CocoDoogy.Audio;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.MiniGame.WindowCleanGame
{
    public class WindowDirty : CanMoveImage
    {
        private WindowCleanMiniGame parent = null;


        void Start()
        {
            SetVisualEmphasize();
            outline.effectDistance = new Vector2(7,7);
        }

        void SetVisualEmphasize()
        {
            transform.DOKill();
            // 스케일 초기화
            transform.localScale = Vector3.one;
            // DoTween 애니메이션 (살짝 튕기는 효과)
            transform.DOScale(1.2f, 0.75f)
                .SetEase(Ease.OutBack)
                .SetLoops(-1, LoopType.Yoyo);
        }


        public void Init(WindowCleanMiniGame parent, Sprite sprite)
        {
            this.parent = parent;
            image.sprite = sprite;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            SfxManager.PlaySfx(SfxType.Minigame_PickLeaf);
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            SetVisualEmphasize();
            List<RaycastResult> raylist = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raylist);
            bool iswindowSlot = false;
            foreach (RaycastResult result in raylist)
            {
                if (result.gameObject == gameObject) continue;
                if (result.gameObject.GetComponent<WindowSlot>())
                {
                    
                    iswindowSlot = true;
                    break;
                }
            }
            //놓는 자리에 WindowSlot없으면 사라짐
            if(!iswindowSlot)
            {
                transform.DOKill();
                
                SfxManager.PlaySfx(SfxType.UI_Success);
                    parent.DestroyDirty(this);
            }
        }
    }
}









