using CocoDoogy.Tile.Gimmick.Data;
using UnityEngine;

namespace CocoDoogy.MapEditor.UI.GimmickConnector.Effect
{
    public abstract class GimmickPageBase : MonoBehaviour
    {
        /// <summary>
        /// 해당 페이지 내에서 작업이 완료되었는지
        /// </summary>
        public abstract bool IsSuccess { get; }

        protected GimmickData SelectedGimmick => Parent.SelectedGimmick;
        protected GimmickConnectPanel Parent { get; private set; } = null;


        /// <summary>
        /// 최초 1회만 동작하는 초기화 메소드
        /// </summary>
        /// <param name="parent"></param>
        public void Init(GimmickConnectPanel parent) => OnInit(Parent = parent);

        /// <summary>
        /// 피상속자가 초기화하기 위해 존재하는 추상 메소드
        /// </summary>
        /// <param name="parent"></param>
        protected abstract void OnInit(GimmickConnectPanel parent);


        /// <summary>
        /// 해당 페이지가 펼쳐졌을 때 동작
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 해당 페이지 닫기
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}