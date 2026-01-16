using UnityEngine;

namespace CocoDoogy.MiniGame.CoatArrangeGame
{
    public class CoatSlot : MonoBehaviour
    {
        public int Id;
        private CoatArrangeMiniGame parent;


        public void Init(CoatArrangeMiniGame coatArrangeMiniGame)
        {
            parent = coatArrangeMiniGame;
            Debug.Log("CoatSlot 추가 전 Count: " + parent.unArrangedCoatSlots.Count);
            parent.unArrangedCoatSlots.Add(this);
            Debug.Log("CoatSlot 추가 후 Count: " + parent.unArrangedCoatSlots.Count);
        }
        
        
        /// <summary>
        /// 슬롯과 코트의 ID를 비교하여 클리어여부를 확인하는 함수
        /// </summary>
        public bool CheckID()
        {
            if (transform.childCount == 0) return false;
            Coat coat = transform.GetChild(0).GetComponent<Coat>();
            if (coat != null && coat.Id == Id)
            {
                // ID가 맞으면 코트 상호작용불가
                coat.SetUnInteractable();
                // unArrangedCoatSlots에서 제거
                parent.unArrangedCoatSlots.Remove(this);
                return true;
            }
            return false;
            // bool correct = (coat != null && coat.Id == Id);
            // if (correct)
            // {
            //     parent.unArrangedCoatSlots.Remove(this);
            //     Destroy(coat);
            // }
            // return correct;
        }
    }
}
