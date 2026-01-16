using CocoDoogy.Audio;
using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame.ToyFindGame
{
    public class Detector : CanMoveImage
    {
        private Tweener outlineTween;
        SandTile lastTile;
        bool isDetecting = true;
        [SerializeField] Image detectorSpritePrefab;
        Coroutine detectingCoroutine;


        private void OnEnable()
        {
            StartDetectingCoroutine(Color.green, 1f);
            
            
        }
        private void OnDisable()
        {
            StopDetectingCoroutine();
            
            DOTween.Kill(transform, complete:false);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            SfxManager.ToggleLoopSound(SfxType.Loop_Detecting);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            SfxManager.ToggleLoopSound(SfxType.Loop_Detecting);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            SandTile hitTile = null;
            foreach (var result in raycastResults)
            {
                if (result.gameObject == gameObject || result.gameObject == null) continue;

                if (result.gameObject.TryGetComponent<SandTile>(out hitTile)) break;
            }

            if (hitTile == lastTile) return;
            lastTile = hitTile;

            if (hitTile == null)
            {
                return;
            }
            if (hitTile.haveToy)
            {
                if (!isDetecting)
                {
                    SfxManager.InitDetectingLevelParameter(3);
                }
                isDetecting = true;
                hitTile.detected = true;
                Handheld.Vibrate();
                StartDetectingCoroutine(Color.indianRed, 0.5f);
            }

            else
            {
                isDetecting = false;
                StartDetectingCoroutine(Color.green, 1f);
                SfxManager.InitDetectingLevelParameter(1);
            }
        }
        #region 보조함수(시각적 효과)

        /// <summary>
        /// 이전 코루틴을 중지시키고 재시작하며 중복방지를 위해 코루틴필드에 넣어두기
        /// </summary>
        /// <param name="color"></param>
        /// <param name="interval"></param>
        void StartDetectingCoroutine(Color color, float interval = 1f)
        {
            StopDetectingCoroutine();
            detectingCoroutine = StartCoroutine(DetectCoroutine(color, interval));
        }


        /// <summary>
        /// 코루틴을 중지시키고 코루틴 필드 비우기
        /// </summary>
        void StopDetectingCoroutine()
        {
            if (detectingCoroutine != null)
            {
                StopCoroutine(detectingCoroutine);
                detectingCoroutine = null;
            }
        }


        /// <summary>
        /// Detecting을 실행시키는 코루틴
        /// </summary>
        /// <param name="color"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        IEnumerator DetectCoroutine(Color color, float interval)
        {
            while (true)
            {
                Detecting(color);
                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>
        /// 레이더를 뿜어내는듯한 시각적 효과
        /// </summary>
        /// <param name="color"></param>
        void Detecting(Color color)
        {
            Image img = LeanPool.Spawn(detectorSpritePrefab, transform);
            img.color = color;
            img.transform.localScale = Vector3.one;
            img.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.SetTarget(img);
            seq.Join(img.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutQuad));
            seq.Join(img.DOFade(0f, 0.4f).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                if(img!= null)
                {
                    seq.Complete(true);
                    DOTween.Kill(img);
                    LeanPool.Despawn(img.gameObject);
                }
            });
        }

       
        #endregion

    }
}
