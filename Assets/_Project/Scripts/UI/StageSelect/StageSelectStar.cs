using CocoDoogy.Network;
using System;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageSelectStar : MonoBehaviour
    {
        private int StarCount = 0;
        [SerializeField] private GameObject[] stageStar;

        private void Awake()
        {
            BrightStar(StarCount);
        }
        
        private void OnDisable()
        {
            Init();
        }
        private void Init()
        {
            foreach (GameObject star in stageStar)
            {
                star.SetActive(false);
            }
        }
        
        public void BrightStar(int star = 1)
        {
            StarCount = star;
            for (int i = 0; i < star; i++)
            {
                stageStar[i].SetActive(true);
            }
        }
    }
}