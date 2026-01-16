using CocoDoogy.Data;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Replay
{
    public class ReplayItemButton : MonoBehaviour
    {
        public CommonButton Button { get; private set; }
        public Image ButtonColor { get; private set; }
        
        /// <summary>
        /// 해당 버튼이 가지고 있는 ItemData를 InGameItemUI에서 넣어줌
        /// </summary>
        public ItemData ItemData { get; set; }
        
        private void Awake()
        {
            if (!Button)
            {
                Button = GetComponent<CommonButton>();
            }

            if (!ButtonColor)
            {
                ButtonColor = GetComponent<Image>();
            }

            // 리플레이 중에 버튼이 눌리면 안되기 때문에 false로
            Button.interactable = false;
        }
    }
}