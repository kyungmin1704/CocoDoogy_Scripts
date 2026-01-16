using CocoDoogy.Audio;
using CocoDoogy.LifeCycle;
using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    /// <summary>
    /// 물 위에 뜨는 상자용
    /// </summary>
    [RequireComponent(typeof(Piece))]
    public class FloatedCratePiece: MonoBehaviour, IInit<Piece>
    {
        [Tooltip("움직일 상자의 Pivot")] [SerializeField] private Transform cratePivot;


        private HexTile Parent => piece?.Parent;


        private Piece piece = null;


        void OnEnable()
        {
            Float();
        }
        void OnDisable()
        {
            DOTween.Kill(this, true);
        }


        public void OnInit(Piece piece)
        {
            this.piece = piece;
        }


        private void Float()
        {
            DOTween.Kill(this, true);
            cratePivot.DOMoveY(-0.1f, 5f).SetLoops(-1, LoopType.Yoyo).SetId(this);
        }


        public void ToMove(HexDirection direction)
        {
            DOTween.Kill(this, true);

            Vector3 offset = Vector3.up * 0.5f;
            cratePivot.position = Parent.GridPos.GetDirectionPos(direction).ToWorldPos() + offset;

            Sequence sequence = DOTween.Sequence();
            sequence.SetId(this);
            sequence.Append(cratePivot.DOMove(Parent.GridPos.ToWorldPos() + offset, Constants.MOVE_DURATION))
                .AppendCallback(()=>SfxManager.PlaySfx(SfxType.Interaction_WaterSplash));
            sequence.Append(cratePivot.DOMoveY(-0.1f, 0.5f).SetEase(Ease.OutBack));
            sequence.Append(cratePivot.DOMoveY(0, 0.2f).SetEase(Ease.OutQuad));
            sequence.OnComplete(Float);
            sequence.Play();
        }
    }
}