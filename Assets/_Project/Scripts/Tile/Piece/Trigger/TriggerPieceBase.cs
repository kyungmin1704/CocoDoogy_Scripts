using CocoDoogy.LifeCycle;
using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    [RequireComponent(typeof(Piece))]
    public abstract class TriggerPieceBase: MonoBehaviour, ISpawn<Piece>, IRelease<Piece>
    {
        /// <summary>
        /// 기물이 올라간 타일
        /// </summary>
        protected HexTile Parent { get; private set; }
        /// <summary>
        /// Trigger가 포함된 기물(= 나)
        /// </summary>
        protected Piece Piece { get; private set; }


        /// <summary>
        /// Trigger의 상태가 On인지 여부
        /// </summary>
        public virtual bool IsOn { get; set; }
        
        
        public void OnSpawn(Piece piece)
        {
            Parent = (Piece = piece).Parent;
        }

        public abstract void OnRelease(Piece data);


        public abstract void Interact();
        public abstract void UnInteract();
    }
}