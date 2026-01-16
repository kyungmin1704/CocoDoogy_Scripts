using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace CocoDoogy.UI
{
    public class Defeat : MonoBehaviour
    {
        [SerializeField] private Sprite brightStarSprite;
        [SerializeField] private Sprite darkStarSprite;
        
        private Image image;
        private RectTransform rectTransform;
        private Vector2 originalScale;

        private void Awake()
        {
            image =  GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }
        
        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            image.sprite = darkStarSprite;
            rectTransform.DOKill();
        }

        private void OnDisable()
        {
            Initialize();
        }
    }
}