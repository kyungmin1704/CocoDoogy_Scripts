using CocoDoogy._Project.Scripts.UI.CocoDoogys;
using CocoDoogy.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI
{
    public class CocoDoogyUIIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private List<ItemSpritesUI> itemSpritesUI;
        

        /// <summary>
        /// 아이콘 변경함수
        /// </summary>
        /// <param name="itemUIType">Struct로 지닌 아이템 타입에 따른 아이콘 모양으로 변한다</param>
        public void ChangeIcon(ItemUIType itemUIType)
        {
            foreach (ItemSpritesUI item in itemSpritesUI)
            {
                if (item.itemUIType == itemUIType)
                {
                    image.sprite = item.sprite;
                }
            }
        }
        
    }
}
