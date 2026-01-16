using Lean.Pool;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Popup
{
    public class InfoDialog : MonoBehaviour
    {
        private static InfoDialog prefab = null;
        private static Transform canvas = null;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI infoText;
        
        [Header("Panels")]
        [SerializeField] private Button backGround;
        
        [Header("Buttons")]
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button useButton;

        [Header("Buttons Text")]
        [SerializeField] private TextMeshProUGUI purchaseButtonText;

        [Header("Info Image")] 
        [SerializeField] private Image icon;
        
        private Action<CallbackType> callback = null;
        private bool hasInit = false;
        
        
        private void Init()
        {
            hasInit = true;
            purchaseButton.onClick.AddListener(OnConfirmOrYesClick);
            useButton.onClick.AddListener(OnConfirmOrYesClick);
            cancelButton.onClick.AddListener(OnNoClick);
            backGround.onClick.AddListener(OnNoClick);
        }
        
        private void Release() => LeanPool.Despawn(this);
        
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

        public static void ShowInfo(string title, string subtitle, string info, Sprite icon, DialogMode type, Action<CallbackType> callback, string buttonText = "구매하기")
        {
            InfoDialog infoDialog = Create();
            
            infoDialog.titleText.text = title;
            infoDialog.subtitleText.text = subtitle;
            infoDialog.infoText.text = info;
            
            infoDialog.icon.sprite = icon;
            
            infoDialog.purchaseButton.gameObject.SetActive(type == DialogMode.Confirm);
            infoDialog.cancelButton.gameObject.SetActive(type == DialogMode.YesNo);
            infoDialog.useButton.gameObject.SetActive(type == DialogMode.YesNo);

            if (type == DialogMode.Confirm)
            {
                infoDialog.purchaseButtonText.text = buttonText;
            }
            infoDialog.callback = callback;
        }


        private static InfoDialog Create()
        {
            if (!canvas)
            {
                canvas = GameObject.Find("PopupCanvas").transform;
            }
            
            InfoDialog result = LeanPool.Spawn(prefab, canvas);
            if (!result.hasInit)
            {
                result.Init();
            }
            result.callback = null;

            return result;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            prefab = Resources.Load<InfoDialog>("UI/InfoDialog");
        }
    }
}