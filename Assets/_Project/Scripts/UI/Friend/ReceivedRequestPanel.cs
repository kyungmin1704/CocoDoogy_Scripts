using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using Lean.Pool;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class ReceivedRequestPanel : RequestPanel
    {
        /// <summary>
        /// 친구 추가를 수락하는 메서드
        /// </summary>
        /// <param name="uid"></param>
        private async void OnAcceptRequestAsync(string uid)
        {
            var result = await FirebaseManager.CallFriendFunctionAsync("receiveFriendsRequest", uid, "친구 요청 수락 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 추가 수락 성공", "해당 유저를 친구 추가 했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 추가 수락 실패", reason, DialogMode.Confirm, null);
            }
        }

        /// <summary>
        /// 친구 추가를 거절하는 메서드
        /// </summary>
        /// <param name="uid"></param>
        private async void OnRejectRequestAsync(string uid)
        {
            var result = await FirebaseManager.CallFriendFunctionAsync("rejectFriendsRequest", uid, "친구 요청 거절 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 추가 거절 성공", "해당 유저를 친구 추가 거절했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 추가 거절 실패", reason, DialogMode.Confirm, null);
            }
        }

        protected override async Task RefreshPanelAsync()
        {
            refreshCts?.Cancel();
            refreshCts = new CancellationTokenSource();
            var token = refreshCts.Token;

            try
            {
                for (int i = container.childCount - 1; i >= 0; i--)
                {
                    Destroy(container.GetChild(i).gameObject);
                }

                var requestDict = await FirebaseManager.GetFriendRequestsAsync("friendReceivedList");
                token.ThrowIfCancellationRequested();

                foreach (var kvp in requestDict)
                {
                    token.ThrowIfCancellationRequested();

                    string uid = kvp.Key;
                    string nickname = kvp.Value;

                    FriendRequestItem item = Instantiate(prefabItem, container);
                    item.ReceivedInit(nickname, uid, OnAcceptRequestAsync, OnRejectRequestAsync);
                }

                bool hasRequests = requestDict.Count > 0;
                nullMessage.gameObject.SetActive(!hasRequests);
                if (!hasRequests)
                    nullMessage.text = "받은 친구 요청이 없습니다.";
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"RefreshPanelAsync 에러 발생: {ex}");
            }
        }
    }
}