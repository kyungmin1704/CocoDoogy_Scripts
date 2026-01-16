using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    public class TutorialMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image background;

        private float defaultalpha = 255f;

    

        /// <summary>
        /// 게임이름과 설명을 매개변수로받으면서 설명창 활성화 (1번이라도 실행되었으면 불가능하게)
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public void ShowTutorialExplain(string desc)
        {
            description.text = desc;
            transform.gameObject.SetActive(true);
            StartCoroutine(InActiveMessage());
        }
        
        private void OnEnable()
        {
            var c2 = description.color;
            c2.a = defaultalpha;
            description.color = c2;

            var c3 = background.color;
            c3.a = defaultalpha;
            background.color = c3;
        }


        /// <summary>
        /// 5초후에 설명창비활성화 
        /// </summary>
        /// <returns></returns>
        IEnumerator InActiveMessage()
        {
            yield return new WaitForSeconds(1f);
            //4초동안 DoTween으로 사라지기
            description.DOFade(0f, 2f);
            background.DOFade(0f, 2f);
            yield return new WaitForSeconds(2.5f);
            transform.gameObject.SetActive(false);
        }
        
        
    }
}
