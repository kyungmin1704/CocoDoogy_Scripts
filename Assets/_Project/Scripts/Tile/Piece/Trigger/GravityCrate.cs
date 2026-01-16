using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    /// <summary>
    /// 상자가 올라간 발판이기 때문에 상시 true 상태
    /// </summary>
    public class GravityCrate: TriggerPieceBase
    {
        [SerializeField] private Transform crate;


        public override bool IsOn => true;
        
        
        public override void OnRelease(Piece data)
        {
            
        }


        public override void Interact()
        {

        }
        public override void UnInteract()
        {
            
        }

        public void ToMove(HexDirection direction)
        {
            crate.position = Parent.GridPos.GetDirectionPos(direction).ToWorldPos();
            crate.DOMove(Parent.GridPos.ToWorldPos(), Constants.MOVE_DURATION).SetId(Piece);
        }
    }
}