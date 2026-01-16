using CocoDoogy.GameFlow.InGame;
using CocoDoogy.LifeCycle;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    /// <summary>
    /// 다리용
    /// </summary>
    [RequireComponent(typeof(Piece))]
    public class BridgePiece : MonoBehaviour, ISpawn<Piece>, IRelease<Piece>
    {
        private HexDirection LookDirection => piece.LookDirection;


        /// <summary>
        /// 이 기물이 올라간 타일
        /// </summary>
        private HexTile Parent
        {
            get => parent;
            set
            {
                if (parent)
                {
                    parent.OnRotateComplete -= OnParentRotateComplete;
                }
                (parent = value).OnRotateComplete += OnParentRotateComplete;
            }
        }


        private HexTile parent = null;
        private Piece piece = null;

        private HexTile frontTile = null;
        private HexTile backTile = null;


        public void OnSpawn(Piece piece)
        {
            Parent = (this.piece = piece).Parent;
            ConnectSideTile();
        }
        public void OnRelease(Piece data)
        {
            if (Parent)
            {
                Parent.OnRotateComplete -= OnParentRotateComplete;
            }
            DisconnectEvents();
        }


        private void ConnectSideTile()
        {
            DisconnectEvents();

            Vector2Int frontPos = Parent.GridPos.GetDirectionPos(LookDirection);
            Vector2Int backPos = Parent.GridPos.GetDirectionPos(LookDirection.GetMirror());

            frontTile = HexTile.GetTile(frontPos);
            backTile = HexTile.GetTile(backPos);

            ConnectEvents();
        }


        private void OnParentRotateComplete(HexTile tile, HexRotate rotate)
        {
            ConnectSideTile();
        }
        private void OnRotated(HexTile tile, HexRotate rotate)
        {
            Vector2Int prePos = Parent.GridPos;
            Quaternion preRot = piece.transform.rotation;
            piece.LookDirection = LookDirection.AddRotate(rotate);
            Parent.Pieces[(int)HexDirection.Center] = null;

            HexDirection directionOfRotateTile = tile == frontTile ? LookDirection.GetMirror() : LookDirection;
            Vector2Int nextParentPos = tile.GridPos.GetDirectionPos(directionOfRotateTile);
            HexTile nextParent = HexTile.GetTile(nextParentPos);
            if (nextParent)
            {
                Parent = HexTile.GetTile(nextParentPos);
                Parent.ConnectPiece(HexDirection.Center, piece);
                piece.transform.position = prePos.ToWorldPos();
                piece.transform.rotation = preRot;
                piece.transform.parent = tile.transform;

                // 플레이어가 다리 위에 있을 때, 같이 회전
                if (PlayerHandler.IsValid && prePos == PlayerHandler.GridPos)
                {
                    DOTween.Kill(PlayerHandler.Instance, true);
                    PlayerHandler.GridPos = Parent.GridPos;
                    PlayerHandler.Instance.transform.parent = piece.transform;
                }
            }
            else
            {
                MessageDialog.ShowMessage("에러 발생", "타일이 없는 곳으로 다리가 회전함", DialogMode.Confirm, null);
                piece.Release();
            }
        }
        private void OnRotateComplete(HexTile tile, HexRotate rotate)
        {
            if (!Parent)
            {
                Debug.LogError("갈 타일이 없다.");
                return;
            }
            Parent.ConnectPiece(HexDirection.Center, piece);

            // 다리가 플레이어와 같이 회전한 뒤 사후 처리
            if (PlayerHandler.IsValid && Parent.GridPos == PlayerHandler.GridPos)
            {
                PlayerHandler.Instance.transform.parent = null;
                DOTween.Kill(PlayerHandler.Instance, true);
            }

            ConnectSideTile();
        }

        private void ConnectEvents()
        {
            if (frontTile)
            {
                frontTile.OnRotateChanged += OnRotated;
                frontTile.OnRotateComplete += OnRotateComplete;
            }
            if (backTile)
            {
                backTile.OnRotateChanged += OnRotated;
                backTile.OnRotateComplete += OnRotateComplete;
            }
        }
        private void DisconnectEvents()
        {
            if (frontTile)
            {
                frontTile.OnRotateChanged -= OnRotated;
                frontTile.OnRotateComplete -= OnRotateComplete;
            }
            if (backTile)
            {
                backTile.OnRotateChanged -= OnRotated;
                backTile.OnRotateComplete -= OnRotateComplete;
            }
        }
    }
}