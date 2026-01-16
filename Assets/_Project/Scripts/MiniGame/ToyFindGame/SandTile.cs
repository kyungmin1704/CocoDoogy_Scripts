using CocoDoogy.MiniGame.ToyFindGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CocoDoogy.Audio;
namespace CocoDoogy.MiniGame.ToyFindGame
{
    public class SandTile : MonoBehaviour, IPointerClickHandler
    {
        public bool haveToy = false;
        private int tileID = -1;
        [SerializeField] GameObject toyPrefab;
        [SerializeField] Sprite[] toySprites;
        [SerializeField] Sprite diggedSprite;
        Image image;
        public bool detected = false;
        bool digged = false; //파진것인지 체크

        private ToyFindMiniGame parent = null;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void Init(ToyFindMiniGame toyFindMiniGame)
        {
            parent = toyFindMiniGame;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (digged) return;
            //모든 레이캐스트를 따와서 레이캐스트에 부딪친게 자신이고 haveToy가 True면 toyPrefab이미지 생성후 랜덤 toySprite로 이미지 교체
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            foreach (var result in raycastResults)
            {
                SfxManager.PlaySfx(SfxType.Minigame_DigSand);
                MiniGameParticleManager.Instance.ParticleDigging(transform.position, transform);
                if (result.gameObject == gameObject && haveToy && detected)
                {
                    GameObject GO = Instantiate(toyPrefab, gameObject.transform);
                    Image toyImage = GO.GetComponent<Image>();
                    toyImage.sprite = toySprites[Random.Range(0, toySprites.Length)];
                    image.sprite = diggedSprite;
                    digged = true;
                    //클리어판단
                    SfxManager.PlaySfx(SfxType.UI_Success);
                    parent.RemoveToy(tileID);
                    parent.CheckClear();
                }
            }
        }

        /// <summary>
        /// Toy가 있도록 함
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tileID"></param>
        public void SetToy(bool value, int tileID)
        {
            haveToy = value;
            this.tileID = tileID;
            if (haveToy && tileID != -1)
            {
                parent.AddToy(tileID);
            }
        }

    }
}
