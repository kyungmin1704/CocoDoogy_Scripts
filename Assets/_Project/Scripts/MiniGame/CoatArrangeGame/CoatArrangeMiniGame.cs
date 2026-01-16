using CocoDoogy.Audio;
using System.Collections.Generic;
using UnityEngine;
using CocoDoogy.Utility;
using UnityEngine.UI;


namespace CocoDoogy.MiniGame.CoatArrangeGame
{
    public class CoatArrangeMiniGame : MiniGameBase
    {
        [SerializeField] private GameObject coatGamePanel;
        [SerializeField] private GameObject coatPrefab;
        [SerializeField] private GameObject coatSlotPrefab;
        [SerializeField] private RectTransform coatSlots;
        [SerializeField] private RectTransform hintPanel;
        [SerializeField] private Sprite[] coatSprites;
        [SerializeField] private Sprite backgroundSprite;
        private int coatCount = 4;
        public List<CoatSlot> unArrangedCoatSlots = new List<CoatSlot>();
        private readonly List<Vector2> initialPositions = new List<Vector2>();
        private readonly List<Sprite> initialSprites = new List<Sprite>();

        //일정한 비율을 유지하기 위해
        private const float SlotWidthRatio = 230f / 2280f;    // 약 0.101
        private const float SlotHeightRatio = 500f / 1080f;   // 약 0.463
        private const float SlotSpacingRatio = 140f / 2280f;  // 약 0.07
        private const float CoatWidthScale = 0.9f;            // 슬롯 대비 코트 크기 비율
        private const float HintSpacingRatio = 550f / 2280f; //힌트패널의 간격
        private const float HintYOffsetRatio = 375f / 1080f;
        

        protected override void ShowRemainCount() { }

        protected override void Disable()
        {
            foreach (Transform child in coatSlots)
            {
                Destroy(child.gameObject);
            }
            foreach(Transform child in hintPanel)
            {
                Destroy(child.gameObject);
            }
        }

        protected override void OnOpenInit()
        {
            unArrangedCoatSlots.Clear();
            SetBackground(backgroundSprite);
            ResizePanel();
            SummonCoatAndCoatSlot();
            ShowHint();
        }

        protected override bool IsClear() => unArrangedCoatSlots.Count <= 0;
   


        protected override void SetBackground(Sprite sprite)
        {
            background.sprite = sprite;
        }

        /// <summary>
        /// UI 패널(windowGamePanel)의 크기를 배경 이미지(background.sprite)의 비율에 맞게 조정하는 함수
        /// 왜 필요하냐. Unity에서 Image 컴포넌트의 Preserve Aspect 옵션을 켜면, 이미지가 원본 비율대로 맞춰지는데
        /// 부모 RectTransform 크기와 다를 수 있기 때문에, 실제 패널 크기도 비율에 맞게 조정해야 화면이 어긋나지 않는다.
        /// </summary>
        protected override void ResizePanel()
        {
            if (background == null || background.sprite == null || coatGamePanel == null)
                return;

            RectTransform panelRect = coatGamePanel.GetComponent<RectTransform>();
            RectTransform bgRect = background.rectTransform;
            Sprite sprite = background.sprite;

            // 현재 Image가 PreserveAspect로 표시될 때 실제 화면에 표시되는 크기 계산
            float spriteRatio = sprite.rect.width / sprite.rect.height;
            float rectRatio = bgRect.rect.width / bgRect.rect.height;

            float width, height;

            if (rectRatio > spriteRatio)
            {
                // 세로 기준 맞춤
                height = bgRect.rect.height;
                width = height * spriteRatio;
            }
            else
            {
                // 가로 기준 맞춤
                width = bgRect.rect.width;
                height = width / spriteRatio;
            }

            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        /// <summary>
        /// 코트와 코트슬롯을 생성하고 코트위치를 셔플함
        /// </summary>
        private void SummonCoatAndCoatSlot()
        {
            initialPositions.Clear();
            initialSprites.Clear();

            foreach (Transform child in coatSlots)
            {
                Destroy(child.gameObject);
            }

            List<Sprite> shuffledSprites = new List<Sprite>(coatSprites);
            //  coatSprites 복사본을 만들어 섞기 (Fisher–Yates Shuffle)
            shuffledSprites.Shuffle();

            // coatCount보다 스프라이트가 적으면 오류 방지
            int usableCount = Mathf.Min(coatCount, shuffledSprites.Count);
            List<GameObject> coatObjects = new List<GameObject>();

            RectTransform panelRect = coatGamePanel.GetComponent<RectTransform>();
            float panelWidth = panelRect.rect.width;
            float panelHeight = panelRect.rect.height;
            float slotWidth = panelWidth * SlotWidthRatio;
            float slotHeight = panelHeight * SlotHeightRatio;

            HorizontalLayoutGroup hLayout = coatSlots.GetComponent<HorizontalLayoutGroup>();
            if (hLayout != null)
            {
                hLayout.spacing = panelWidth * SlotSpacingRatio;
            }

            for (int i = 0; i < coatCount; i++)
            {
                //코트슬롯 생성
                GameObject coatSlotObj = Instantiate(coatSlotPrefab, coatSlots);
                RectTransform slotRect = coatSlotObj.GetComponent<RectTransform>();
                slotRect.sizeDelta = new Vector2(slotWidth, slotHeight);

                CoatSlot coatSlot = coatSlotObj.GetComponent<CoatSlot>();
                coatSlot.Id = i;
                coatSlot.Init(this);

                //코트 생성
                GameObject coatObj = Instantiate(coatPrefab, coatSlot.transform);
                Coat coat = coatObj.GetComponent<Coat>();
                coat.Id = i;
                coat.Init(this);

                Image image = coat.GetComponent<Image>();
                image.sprite = shuffledSprites[i];

                RectTransform coatRect = coat.GetComponent<RectTransform>();    
                //셔플된 스프라이트에서 사용
                float ratio = image.sprite.rect.height / image.sprite.rect.width;
                float coatWidth = slotWidth * 0.9f;
                float coatHeight = coatWidth * ratio;
                coatRect.sizeDelta = new Vector2(coatWidth, coatHeight);


                //힌트를 주기 위해 초기 위치와 스프라이트 저장
                initialPositions.Add(coatObj.GetComponent<RectTransform>().anchoredPosition);
                initialSprites.Add(shuffledSprites[i]);
                coatObjects.Add(coatObj);
            }

            List<GameObject> suffledCoats = new List<GameObject>(coatObjects);

            //섞었음에도 완전히 같은 위치에 있을수도 있다.
            //이를 막기 위해 완전히 같은 위치에 있을경우 다시 섞도록 한다.
            bool allInSamePlace;
            do
            {
                suffledCoats.Shuffle();
                allInSamePlace = true;
                for(int i = 0;i < suffledCoats.Count;i++)
                {
                    if(suffledCoats[i] != coatObjects[i])
                    {
                        allInSamePlace = false;
                        break;
                    }
                }
            } while(allInSamePlace);

            for (int i = 0; i < suffledCoats.Count; i++)
            {
                suffledCoats[i].transform.SetParent(coatSlots.GetChild(i), false);
            }

            CoatSlotsCheck();
        }

        void CoatSlotsCheck()
        {
            for (int i = unArrangedCoatSlots.Count - 1; i >= 0; i--)
            {
                CoatSlot coatslot = unArrangedCoatSlots[i];
                coatslot.CheckID();
                print($"{coatslot.Id}번째 코트슬롯 ID 체크");
            }
            CheckClear();
        }

        /// <summary>
        /// 위치가 바뀐 코트들의 ID를 검사하고 클리어 조건을 검사한다.
        /// </summary>
        /// <param name="slotA"></param>
        /// <param name="slotB"></param>
        public void OnCoatSwapped(CoatSlot slotA, CoatSlot slotB)
        {
            if (slotA != null&& slotA.CheckID())
            {
                MiniGameParticleManager.Instance.ParticleCoat(slotA.transform.position, slotA.transform);
                SfxManager.PlaySfx(SfxType.UI_Success);
            }
            
            if(slotB != null&& slotB.CheckID())
            {
                MiniGameParticleManager.Instance.ParticleCoat(slotB.transform.position, slotB.transform);
                SfxManager.PlaySfx(SfxType.UI_Success);
            }
            CheckClear();
        }

        /// <summary>
        /// 힌트를 생성하고 보여준다.
        /// </summary>
        void ShowHint()
        {
            hintPanel.gameObject.SetActive(true);

            RectTransform panelRect = coatGamePanel.GetComponent<RectTransform>();
            float panelWidth = panelRect.rect.width;
            float panelHeight = panelRect.rect.height;

            float slotWidth = panelWidth * SlotWidthRatio;
            float slotHeight = panelHeight * SlotHeightRatio;
            float spacing = panelWidth * HintSpacingRatio;

            Vector2 hintPos = hintPanel.anchoredPosition;
            hintPos.y = panelHeight * HintYOffsetRatio;
            hintPanel.anchoredPosition = hintPos;

            HorizontalLayoutGroup hintLayout = hintPanel.GetComponent<HorizontalLayoutGroup>();
            if(hintLayout != null)
            {
                hintLayout.spacing = spacing;
            }

            //힌트 패널에 초기 위치와 스프라이트로 코트 이미지 생성
            //coatGamePanel에 있는 Background 오브젝트를 힌트패널의 자식으로 추가
            for (int i = 0; i < initialPositions.Count; i++)
            {
                GameObject hintCoatObj = new GameObject("HintCoat" + i, typeof(RectTransform), typeof(Image));
                hintCoatObj.transform.SetParent(hintPanel, false);
                RectTransform rt = hintCoatObj.GetComponent<RectTransform>();
                rt.anchoredPosition = initialPositions[i];
                Image img = hintCoatObj.GetComponent<Image>();
                img.sprite = initialSprites[i];

                float ratio = img.sprite.rect.height / img.sprite.rect.width;
                float width = slotWidth*CoatWidthScale;
                float height = width * ratio;

                rt.sizeDelta = new Vector2(width, height);
            }
        }
    }
}
