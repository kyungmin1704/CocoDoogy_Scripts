using CocoDoogy.CameraSwiper;
using UnityEngine;

namespace CocoDoogy.UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// UIPanel을 상속받은 컴포넌트를 가진 오브젝트의 Active 상태를 true 변경
        /// virtual로 만들어서 따로 수정할 내용이 없으면 그대로 사용
        /// </summary>
        public virtual void OpenPanel()
        {
            gameObject.SetActive(true);
            PageCameraSwiper.IsSwipeable = false;
        }


        /// <summary>
        /// UIPanel을 상속받은 컴포넌트를 가진 오브젝트의 Active 상태를 false 변경 <br/>
        /// abstract로 만들어서 각 컴포넌트 내에서 상속받아서 사용
        /// </summary>
        public abstract void ClosePanel();
    }
}