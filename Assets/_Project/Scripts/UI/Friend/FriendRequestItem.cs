using CocoDoogy.Data;
using CocoDoogy.Network;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Friend
{
    public class FriendRequestItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI recordText;
        
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button presentButton;
        [SerializeField] private Button deleteButton;
        
        private Action<string> onAccept;
        private Action<string> onReject;
        private Action<string> onCancel;
        private Action<string> onDelete;
        private Action<string> onPresent;
        private string uid;
        
        /// <summary>
        /// Received Request에 사용하는 초기화 메서드
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="receivedUid"></param>
        /// <param name="acceptCallback"></param>
        /// <param name="rejectCallback"></param>
        public async void ReceivedInit(string nickname, string receivedUid, Action<string> acceptCallback, Action<string> rejectCallback)
        {
            uid = receivedUid;
            StageInfo record = await FirebaseManager.GetLastClearStage(uid);
            
            nicknameText.text = nickname;
            recordText.text = $"{(record != null ? record.theme.Hex2Int():"X" )} 테마 {(record != null ? record.level.Hex2Int():"X")} 스테이지";
            onAccept = acceptCallback;
            onReject = rejectCallback;

            acceptButton.onClick.AddListener(() => onAccept?.Invoke(uid));
            rejectButton.onClick.AddListener(() => onReject?.Invoke(uid));
        }

        /// <summary>
        /// Sent Request에 사용하는 초기화 메서드
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="sentUid"></param>
        /// <param name="cancelCallback"></param>
        public async void SentInit(string nickname, string sentUid, Action<string> cancelCallback)
        {
            uid = sentUid;
            StageInfo record = await FirebaseManager.GetLastClearStage(uid);
            
            nicknameText.text = nickname;
            recordText.text = $"{(record != null ? record.theme.Hex2Int():"X" )} 테마 {(record != null ? record.level.Hex2Int():"X")} 스테이지";
            onCancel = cancelCallback;

            cancelButton.onClick.AddListener(() => onCancel?.Invoke(sentUid));
        }

        public async void FriendInit(string nickname, string uid, Action<string> presentCallback, Action<string> cancelCallback)
        {
            this.uid = uid;
            StageInfo record = await FirebaseManager.GetLastClearStage(uid);
            
            nicknameText.text = nickname;
            recordText.text = $"{(record != null ? record.theme.Hex2Int():"X" )} 테마 {(record != null ? record.level.Hex2Int():"X")} 스테이지";
            onPresent = presentCallback;
            onDelete = cancelCallback;

            presentButton.onClick.AddListener(() => onPresent?.Invoke(this.uid));
            deleteButton.onClick.AddListener(() => onDelete?.Invoke(this.uid));
        }
    }
}