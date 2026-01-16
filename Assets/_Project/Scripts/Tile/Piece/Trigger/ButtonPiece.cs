using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Phase.Passage;
using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    /// <summary>
    /// Button용 트리거<br/>
    /// 동작 이후 몇 행동력 뒤에 장비를 정지함
    /// </summary>
    public class ButtonPiece : TriggerPieceBase, ISpecialPiece
    {
        [SerializeField] private Transform buttonObject;


        private int actionPoints = 0;


        public override void OnRelease(Piece data)
        {
            buttonObject.localPosition = Vector3.zero;
        }
        

        public override void Interact()
        {
            buttonObject.localPosition = Vector3.down * 0.2f;
            IsOn = true;

            SfxManager.PlaySfx(SfxType.Interaction_SwitchOn);
            int actionPoints = InGameManager.ConsumedActionPoints + this.actionPoints;
            print(actionPoints);
            InGameManager.Passages.Add(new ButtonPassage(actionPoints, Parent.GridPos));
        }
        public override void UnInteract()
        {
            buttonObject.localPosition = Vector3.zero;
            IsOn = false;

            SfxManager.PlaySfx(SfxType.Interaction_SwitchOff);
            
            bool isDelete = false;
            for (int i = 0;i < InGameManager.Passages.Count;i++)
            {
                PassageBase passage = InGameManager.Passages[i];
                
                if (passage is not ButtonPassage buttonPassage) continue;
                if(buttonPassage.GridPos != Parent.GridPos) continue;
                InGameManager.Passages.RemoveAt(i);
                break;
            }
        }

        public void OnDataInsert(string data)
        {
            if (!int.TryParse(data, out int num)) return;
            
            actionPoints = num;
        }

        public void OnExecute()
        {
        }
    }
}