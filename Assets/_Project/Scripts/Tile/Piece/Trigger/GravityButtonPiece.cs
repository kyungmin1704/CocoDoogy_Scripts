using CocoDoogy.Audio;
using UnityEngine;
using CocoDoogy.GameFlow.InGame;

namespace CocoDoogy.Tile.Piece.Trigger
{
    /// <summary>
    /// 발판 기물은 위에 플레이어가 올라갔는지만 체크함
    /// </summary>
    public class GravityButtonPiece: TriggerPieceBase
    {
        public override bool IsOn => PlayerHandler.GridPos == Parent.GridPos;
        
        
        public override void OnRelease(Piece data)
        {
            
        }


        public override void Interact()
        {
            SfxManager.PlaySfx(SfxType.Interaction_PressurePlate);
        }
        public override void UnInteract()
        {
            
        }
    }
}