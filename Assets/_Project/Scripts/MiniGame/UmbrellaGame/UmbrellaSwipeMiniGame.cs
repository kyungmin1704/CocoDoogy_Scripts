using CocoDoogy.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct UmbrellaSpritePair
{
    public Sprite wetSprite;
    public Sprite drySprite;
}

namespace CocoDoogy.MiniGame.UmbrellaGame
{
    public class UmbrellaSwipeMiniGame : MiniGameBase
    {
        [SerializeField] private GameObject umbrellaParent;
        [SerializeField] private GameObject umbrellaPrefab;
        [SerializeField] private Sprite backgroundSprite;
        //우산을 흔들고 클리어하는 로직을 짰는데 처음엔 우산흔들기가 끝나면 우산이 자체적으로 이미지를 변경시키게 하려고 했으나
        //우산 이미지의 통일성을 지키려면 umbrellaMaker한테 받은 젖은 우산의 이미지의 마른 버전을 함께 참조해아 하는 문제가 생김
        //순서를 잘지키면 문제가 없겠으나 실수하기 쉬움.
        //>>해결법 구조체를 통해 Umbrella에게 젖은 우산 이미지와 마른 우산 이미지를 묶어서 같이 넘겨주기
        [Header("우산 스프라이트 쌍")]
        [SerializeField] private List<UmbrellaSpritePair> umbrellaSpritePairs = new List<UmbrellaSpritePair>();

        [SerializeField] private int summonUmbrellaCount = 1;
        public List<Umbrella> clearcount = new List<Umbrella>();
        private int allcount; //clearCount안에 있는 각 우산의 필요한 needSwipeCount를 합쳐서 UI에 표시하기 위한 매개체

        protected override void OnOpenInit()
        {
            SetBackground(backgroundSprite);
            allcount = 0;
            clearcount.Clear();
            SummonUmbrellas();
        }
        protected override bool IsClear() => clearcount.Count <= 0;

        protected override void ShowRemainCount()
        {
            //private인 umbrella의 needSwipeCount를 여기다가 옮길수 있을까 public밖에 없는걸까나
            foreach (var umbrella in clearcount)
            {
                allcount = umbrella.needSwipeCount;
            }
            remainCount.text = "남은 흔들기: "+allcount.ToString();
        }

        protected override void Disable()
        {
            foreach(Umbrella umbrella in umbrellaParent.GetComponentsInChildren<Umbrella>())
            {
                Destroy(umbrella.gameObject);
            }
        }

        protected override void ResizePanel()
        {
            
        }

        /// <summary>
        /// 배경 바꾸기
        /// </summary>
        /// <param name="sprite"></param>
        protected override void SetBackground(Sprite sprite)
        {
            background.sprite = sprite;
        }

        /// <summary>
        /// 우산 소환메서드 umbrellaparent의 크기에 맞게 랜덤 위치
        /// </summary>
        private void SummonUmbrellas()
        {
            List<UmbrellaSpritePair> shffuledPairs = new List<UmbrellaSpritePair>(umbrellaSpritePairs);
            shffuledPairs.Shuffle();

            RectTransform rect = umbrellaParent.GetComponent<RectTransform>();
            Vector2 parentSize = rect.rect.size;//부모의 rect크기
            //자식이 생성될 랜덤 위치

            //우산 위치를 정확히 어떻게 할지//캔버스 내 랜덤위치생성
            for(int i = 0; i < summonUmbrellaCount; i++)
            {
                float X = Random.Range(-parentSize.x / 2f, parentSize.x / 2f);
                float Y = Random.Range(-parentSize.y / 2f, parentSize.y / 2f);
                GameObject wet = Instantiate(umbrellaPrefab, umbrellaParent.transform);
                RectTransform wetPos = wet.GetComponent<RectTransform>();
                Umbrella wetUmbrella = wet.GetComponent<Umbrella>();
                clearcount.Add(wetUmbrella);
                wetUmbrella.Init(this);
                wetPos.anchoredPosition = new Vector2(X, Y);

                var pair = shffuledPairs[i%shffuledPairs.Count];
                wetUmbrella.SetSprites(pair.wetSprite, pair.drySprite);
            }
        }
    }
}
