using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    /// <summary>
    /// Tile 내 상호작용 가능한 Piece용 interface
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 현재 사용 가능 여부
        /// </summary>
        public bool CanInteract { get; }

        /// <summary>
        /// 상호작용 버튼에 들어가야할 Icon
        /// </summary>
        public Sprite Icon { get; }

        /// <summary>
        /// 상호작용 동작
        /// </summary>
        public void OnInteractClicked();
    }
}