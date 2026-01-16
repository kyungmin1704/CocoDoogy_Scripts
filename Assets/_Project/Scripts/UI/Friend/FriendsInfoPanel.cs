using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class FriendsInfoPanel : RequestPanel
    {
        private async void OnDeleteRequestAsync(string uid)
        {
            IDictionary<string, object> result = await FirebaseManager.CallFriendFunctionAsync("deleteFriendsRequest", uid, "친구 삭제 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 삭제 성공", "친구 삭제에 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.TryGetValue("reason", out object value) ? value.ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 삭제 실패", reason, DialogMode.Confirm, null);
            }
        }

        private async void OnGiftRequestAsync(string uid)
        {
            IDictionary<string, object> result = await FirebaseManager.CallFriendFunctionAsync("giftFriendsRequest", uid, "선물 보내기 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("선물 보내기 성공", "선물 보내기를 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.TryGetValue("reason", out object value) ? value.ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("선물 보내기 실패", reason, DialogMode.Confirm, null);
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

                Dictionary<string, string> requestDict = await FirebaseManager.GetFriendRequestsAsync("friendsList");
                token.ThrowIfCancellationRequested();

                foreach (var kvp in requestDict)
                {
                    token.ThrowIfCancellationRequested();

                    string uid = kvp.Key;
                    string nickname = kvp.Value;

                    FriendRequestItem item = Instantiate(prefabItem, container);
                    item.FriendInit(nickname, uid, OnGiftRequestAsync, OnDeleteRequestAsync);
                }

                bool hasFriends = requestDict.Count > 0;
                nullMessage.gameObject.SetActive(!hasFriends);
                if (!hasFriends)
                    nullMessage.text = "등록된 친구가 없습니다.";
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Debug.LogError($"RefreshFriendListAsync 에러 발생: {ex}");
            }
        }
    }
}