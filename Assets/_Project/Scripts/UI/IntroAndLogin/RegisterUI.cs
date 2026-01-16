using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.IntroAndLogin
{
    public class RegisterUI : UIPanel
    {
        [SerializeField] private TMP_InputField nicknameInputField;
        [SerializeField] private MessagePopup errorPopup;
        [SerializeField] private MessagePopup createPopup;
        [SerializeField] private Button confirmButton;

        public MessagePopup ErrorPopup => errorPopup;
        public MessagePopup CreatePopup => createPopup;

        public override void ClosePanel() => gameObject.SetActive(false);

        public Task<string> InputNicknameAsync()
        {
            OpenPanel();
            var taskCompletionSource = new TaskCompletionSource<string>();

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                taskCompletionSource.TrySetResult(nicknameInputField.text);
            });

            return taskCompletionSource.Task;
        }
    }
}