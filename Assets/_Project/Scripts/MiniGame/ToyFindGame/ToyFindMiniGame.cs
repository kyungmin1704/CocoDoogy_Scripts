using System.Collections.Generic;
using UnityEngine;
using CocoDoogy.Utility;

namespace CocoDoogy.MiniGame.ToyFindGame
{
public class ToyFindMiniGame : MiniGameBase
{
        [SerializeField] private Sprite baackground;//억지배경

        [SerializeField] private RectTransform sandGamePanel;
        [SerializeField] private int toyCount = 3;
        [SerializeField] private SandTile sandTilePrefab;
        [SerializeField] private RectTransform sandTileParent;

        [SerializeField] private int columns = 4;
        [SerializeField] private int rows = 3;

        [SerializeField] private Detector detector;

        //needClear의 수만큼 무작위 Tile을 hasToy로 만든다.

        private List<SandTile> spawnedTile = new List<SandTile>();

        public List<int> toies = new List<int>();


        protected override void ShowRemainCount()
        {
            remainCount.gameObject.SetActive(true);
            remainCount.text = toies.Count.ToString()+$"/{toyCount}";
        }

        protected override void OnOpenInit()
        {
            SetBackground(baackground);
            ResizePanel();
            SummonSandTiles();
            HideToys();
        }

        protected override void Disable()
        {
            remainCount.gameObject.SetActive(false);
            spawnedTile.Clear();
            foreach(Transform child in sandTileParent)
            {
                Destroy(child.gameObject);
            }
        }

        protected override void SetBackground(Sprite sprite)
        {
            //얘는 배경을 생성하는 애라 필요가 없었으나 혼자만 화면을 꽉채우는 것은 일관성이 지켜지지 않고 해상도도 일그러지므로 배경을 참조하도록 한다.
            background.sprite = sprite;
        }

        /// <summary>
        /// UI 패널(windowGamePanel)의 크기를 배경 이미지(background.sprite)의 비율에 맞게 조정하는 함수
        /// 왜 필요하냐. Unity에서 Image 컴포넌트의 Preserve Aspect 옵션을 켜면, 이미지가 원본 비율대로 맞춰지는데
        /// 부모 RectTransform 크기와 다를 수 있기 때문에, 실제 패널 크기도 비율에 맞게 조정해야 화면이 어긋나지 않는다.
        /// </summary>
        protected override void ResizePanel()
        {
            if (background == null || background.sprite == null || sandGamePanel == null)
                return;

            RectTransform panelRect = sandGamePanel.GetComponent<RectTransform>();
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

        protected override bool IsClear() => toies.Count <= 0;
        

        /// <summary>
        /// 타일생성
        /// sandTileParent의 전체크기를 columns, row만큼 잘라 타일을 배치한다.
        /// </summary>
        void SummonSandTiles()
        {
            toies.Clear();
            spawnedTile.Clear();

            //부모의 크기를 기준으로
            Vector2 parentSize = sandTileParent.rect.size;

            //잘라
            float cellWidth = parentSize.x / columns;
            float cellHeight = parentSize.y / rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    SandTile tileObj = Instantiate(sandTilePrefab, sandTileParent);
                    RectTransform rect = tileObj.GetComponent<RectTransform>();

                    rect.sizeDelta = new Vector2(cellWidth, cellHeight);

                    float posX = (x + 0.5f) * cellWidth - parentSize.x / 2f;
                    float posY = -(y + 0.5f) * cellHeight + parentSize.y / 2f;
                    rect.anchoredPosition = new Vector2(posX, posY);

                    SandTile tile = tileObj.GetComponent<SandTile>();
                    tile.Init(this);
                    spawnedTile.Add(tile);
                }
            }
            Instantiate(detector, sandTileParent);
        }

        /// <summary>
        /// 생성된 타일중 몇개를 골라 Toy를 배치하게한다.
        /// </summary>
        private void HideToys()
        {
            spawnedTile.Shuffle();
            for (int i = 0; i < toyCount; i++)
            {
                spawnedTile[i].SetToy(true,i);
            }
            for (int i = toyCount; i < spawnedTile.Count; i++)
            {
                spawnedTile[i].SetToy(false,i);
            }
        }

        /// <summary>
        ///  클리어조건이 될 List에 Toy를 담는다.
        /// </summary>
        /// <param name="tileID"></param>
        public void AddToy(int tileID)
        {
            if (!toies.Contains(tileID))
            {
                toies.Add(tileID);
            }
        }

        /// <summary>
        /// Toy를 발견시 List에서 제거하게 한다.
        /// </summary>
        /// <param name="tileID"></param>
        public void RemoveToy(int tileID)
        {
            if (toies.Contains(tileID))
            {
                toies.Remove(tileID);
            }
        }
    }
}
