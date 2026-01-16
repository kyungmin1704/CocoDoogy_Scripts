using System;
using UnityEngine;

namespace CocoDoogy.UI.Popup
{
    [Serializable]
    public class GameEndWindow
    {
        [Header("Title Elements")]
        public Sprite titleSprite;
        public Sprite titleTextSprite;

        [Header("Score Elements")] 
        public GameObject scoreField;
        
        [Header("Effect Element")]
        public Sprite effectBackground;
        public Sprite effectText;
    }
}