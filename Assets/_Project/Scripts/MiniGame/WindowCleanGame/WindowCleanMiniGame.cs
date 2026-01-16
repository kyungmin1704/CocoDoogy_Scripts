using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame.WindowCleanGame
{
    public class WindowCleanMiniGame : MiniGameBase
    {
        [SerializeField] private int dirtyCount = 5;

        [SerializeField] private RectTransform windowGamePanel;
        [SerializeField] private WindowDirty dirtyPrefab;
        [SerializeField] private RectTransform dirtyGroup;

        [SerializeField] Sprite[] forestDirties;
        [SerializeField] Sprite[] sandDirties;
        [SerializeField] Sprite[] waterDirties;
        [SerializeField] Sprite[] snowDirties;

        [SerializeField] private Sprite forestbackgroundSprite;
        [SerializeField] private Sprite sandbackgroundSprite;
        [SerializeField] private Sprite waterbackgroundSprite;
        [SerializeField] private Sprite snowbackgroundSprite;


        private readonly List<WindowDirty> dirties = new();
        private Dictionary<Theme, Sprite[]> dirtiesDict;
        private Dictionary<Theme, Sprite> backgroundDict;
        
        private void Awake()
        {
            dirtiesDict = new Dictionary<Theme, Sprite[]>
            {
                {Theme.Forest, forestDirties},
                {Theme.Sand, sandDirties},
                { Theme.Water, waterDirties},
                {Theme.Snow, snowDirties},
            };
            backgroundDict = new Dictionary<Theme, Sprite>
            {
                {Theme.Forest,forestbackgroundSprite },
                {Theme.Sand, sandbackgroundSprite },
                { Theme.Water, waterbackgroundSprite },
                {Theme.Snow, snowbackgroundSprite },
            };
            
        }
        protected override bool IsClear() => dirties.Count <= 0;

        protected override void ShowRemainCount()
        {
            remainCount.gameObject.SetActive(true);
            remainCount.text = dirties.Count.ToString()+$"/{dirtyCount}";
        }
        
        protected override void Disable()
        {
            remainCount.gameObject.SetActive(false);
            dirties.Clear();
            foreach(WindowDirty obj in dirtyGroup.GetComponentsInChildren<WindowDirty>())
            {
                Destroy(obj.gameObject);
            }
        }

        /// <summary>
        /// 테마받기
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        Sprite BackgroundFromTheme(Theme theme)
        {
            backgroundDict.TryGetValue(theme, out Sprite sprite);
            return sprite;
        }

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
            if (background == null || background.sprite == null || windowGamePanel == null)
                return;

            RectTransform panelRect = windowGamePanel;
            RectTransform bgRect = background.rectTransform;
            Sprite sprite = background.sprite;

            // 현재 Image가 PreserveAspect로 표시될 때 실제 화면에 표시되는 크기 계산
            float spriteRatio = sprite.rect.width / sprite.rect.height;
            float rectRatio = bgRect.rect.width / bgRect.rect.height;

            float width, height;

            //배경패널이 사용 이미지보다 너비가 더 넓으면 세로기준 맞춤
            if (rectRatio > spriteRatio)
            {
                // 세로 기준 맞춤
                height = bgRect.rect.height;
                width = height * spriteRatio;
            }
            //배경패널이 사용 이미지보다 높이가 크면 가로기준 맞춤
            else
            {
                // 가로 기준 맞춤
                width = bgRect.rect.width;
                height = width / spriteRatio;
            }

            //계산한 width, height를 스트레치된 패널에 적용
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        protected override void OnOpenInit()
        {
            Theme nowTheme = InGameManager.Stage.theme;   //TODO: 나중에 맵 데이터에서 호출하게 변경
            //테마받고 배경바꾸기
            SetBackground(BackgroundFromTheme(nowTheme));
            ResizePanel();
            dirties.Clear();
            Vector3[] corners = new Vector3[4];
            dirtyGroup.GetWorldCorners(corners);

            Vector2 slotStartPoint = corners[0];
            Vector2 slotEndPoint = corners[2];


            Sprite[] dirtySprites = dirtiesDict[nowTheme];
            for(int i = 0;i < dirtyCount; i++)
            {
                float x = Random.Range(slotStartPoint.x, slotEndPoint.x);
                float y = Random.Range(slotStartPoint.y, slotEndPoint.y);

                Sprite sprite = dirtySprites[Random.Range(0, dirtySprites.Length)];
                var obj = Instantiate(dirtyPrefab, new Vector2(x, y), Quaternion.identity, dirtyGroup);
                obj.Init(this, sprite);

                dirties.Add(obj);

            }
            //클리어까지 남은 숫자카운트를 위한 세팅과 옵저버패턴으로 UI상에서 카운트를 구현했습니다.
            // ShowRemainCount();
            // remainCountCallback = ShowRemainCount;
        }

        /// <summary>
        /// 객체를 사라지게 하고 클리어 조건을 검사함
        /// </summary>
        /// <param name="dirty"></param>
        public void DestroyDirty(WindowDirty dirty)
        {
            dirties.Remove(dirty);
            Image img = dirty.GetComponent<Image>();
            img.DOFade(0f, 0.5f).OnComplete(() => 
            {
                Destroy(dirty.gameObject);
                CheckClear();
            });

            
        }
    }
}
