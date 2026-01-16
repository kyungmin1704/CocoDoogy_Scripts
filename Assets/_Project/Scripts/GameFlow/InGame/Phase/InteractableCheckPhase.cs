using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    public class InteractableCheckPhase: IPhase
    {
        private Vector2Int? gridPos = null;
        private Vector2Int destination = Vector2Int.zero;
        

        public void OnClear()
        {
            gridPos = null;
            destination = Vector2Int.zero;
        }
        
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;
            
            InGameManager.ChangeInteract(null, false, null);

            // 타일 존재 확인
            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (!tile) return false;

            for(int i = 0 ;i < tile.Pieces.Length;i++)
            {
                Piece piece = tile.Pieces[i];
                if (!piece) continue;

                if (!piece.TryGetComponent(out IInteractable interactable)) continue;
                InGameManager.ChangeInteract(interactable.Icon, interactable.CanInteract, interactable.OnInteractClicked);
                break;
            }

            return true;
        }
    }
}