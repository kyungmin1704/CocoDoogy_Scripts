using CocoDoogy.Audio;
using Lean.Pool;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Popup
{
    public class MessageDialog : MonoBehaviour
    {
        private static MessageDialog prefab = null;
        private static Transform canvas = null;


        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Panels")]
        [SerializeField] private GameObject buttonsPanel;
        [SerializeField] private GameObject inputFieldPanel;
        [SerializeField] private Button backGround;

        [Header("Message Mode Buttons")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private Button cancelButton;

        [Header("InputField Mode Buttons")]
        [SerializeField] private Button inputConfirmButton;
        [SerializeField] private Button inputCancelButton;

        [Header("InputField")]
        [SerializeField] private TMP_InputField inputText;


        private Action<CallbackType> callback = null;
        private Action<string> strCallback = null;
        private bool hasInit = false;


        private void Init()
        {
            hasInit = true;

            yesButton.onClick.AddListener(OnConfirmOrYesClick);
            noButton.onClick.AddListener(OnNoClick);
            backGround.onClick.AddListener(OnNoClick);
            
            cancelButton.onClick.AddListener(OnCancelClick);

            inputConfirmButton.onClick.AddListener(OnInputConfirmClick);
            inputCancelButton.onClick.AddListener(OnInputCancelClick);
        }

        private void Release()
        {
            LeanPool.Despawn(this);
        }


        private void OnConfirmOrYesClick()
        {
            callback?.Invoke(CallbackType.Yes);
            Release();
        }
        private void OnNoClick()
        {
            callback?.Invoke(CallbackType.No);
            Release();
        }
        private void OnCancelClick()
        {
            callback?.Invoke(CallbackType.Cancel);
            Release();
        }

        private void OnInputConfirmClick()
        {
            //strCallback을 실행
            strCallback?.Invoke(inputText.text);
            Release();
        }
        private void OnInputCancelClick()
        {
            strCallback?.Invoke(null);
            Release();
        }


        /// <summary>
        /// callback에 값을 넣어주면서 패널을 띄우는 함수
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public static MessageDialog ShowMessage(string title, string message, DialogMode type, Action<CallbackType> callback)
        {
            MessageDialog messageDialog = Create();
            messageDialog.titleText.text = title;
            messageDialog.messageText.text = message;

            messageDialog.buttonsPanel.SetActive(true);
            messageDialog.inputFieldPanel.SetActive(false);

            messageDialog.yesButton.gameObject.SetActive(true);
            messageDialog.noButton.gameObject.SetActive(type is DialogMode.YesNo or DialogMode.YesNoCancel);
            messageDialog.cancelButton.gameObject.SetActive(type == DialogMode.YesNoCancel);

            messageDialog.callback = callback;

            SfxManager.PlaySfx(SfxType.UI_PopUp);
            
            return messageDialog;
        }
        /// <summary>
        /// strCallback에 값을 넣어주면서 패널을 띄우는 함수
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="strCallback"></param>
        public static void ShowInputBox(string title, string message, Action<string> strCallback)
        {
            MessageDialog messageDialog = Create();
            messageDialog.titleText.text = title;
            messageDialog.messageText.text = message;

            messageDialog.buttonsPanel.SetActive(false);
            messageDialog.inputFieldPanel.SetActive(true);

            messageDialog.inputConfirmButton.gameObject.SetActive(true);

            messageDialog.strCallback = strCallback;

            SfxManager.PlaySfx(SfxType.UI_PopUp);
        }

        private static MessageDialog Create()
        {
            if (!canvas)
            {
                canvas = GameObject.Find("PopupCanvas").transform;
            }

            MessageDialog result = LeanPool.Spawn(prefab, canvas);
            if (!result.hasInit)
            {
                result.Init();
            }
            result.callback = null;
            result.strCallback = null;

            return result;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            prefab = Resources.Load<MessageDialog>("UI/MessageDialog");
        }
    }
}
