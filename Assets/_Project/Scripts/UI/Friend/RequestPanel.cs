using CocoDoogy.Network;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public abstract class RequestPanel : MonoBehaviour
    {
        [SerializeField] protected Transform container;
        [SerializeField] protected FriendRequestItem prefabItem;
        [SerializeField] protected TextMeshProUGUI nullMessage;

        protected CancellationTokenSource refreshCts;
        
        public void Refresh() => _ = RefreshPanelAsync();
        /// <summary>
        /// 친구 요청(유저), 친구 요청 취소(유저), 친구 요청 거절(상대방), 친구 요청 수락(양쪽)의 경우 UI를 새로고침하여 현재 상황에 맞게 변경
        /// </summary>
        protected abstract Task RefreshPanelAsync();
    }
}