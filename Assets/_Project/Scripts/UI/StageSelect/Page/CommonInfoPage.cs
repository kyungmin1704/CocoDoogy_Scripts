using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Page
{
    /// <summary>
    /// 스테이지 상세보기
    /// </summary>
    public class CommonInfoPage : StageInfoPage
    {
        [SerializeField] private GameObject[] clearStars;


#if UNITY_EDITOR
        void Reset()
        {
            List<GameObject> stars = new();
            foreach (Transform child in transform.Find("Stars"))
            {
                stars.Add(child.GetChild(0).gameObject);
            }
            clearStars = stars.ToArray();
        }
#endif

        protected override void OnShowPage()
        {

        }
    }
}