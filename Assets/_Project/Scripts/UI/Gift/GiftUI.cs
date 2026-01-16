using CocoDoogy.CameraSwiper;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CocoDoogy.UI.Gift
{
    public class GiftUI : UIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button closeThisButton;
        [SerializeField] private CommonButton getAllButton;

        [Header("UI Elements")]
        [SerializeField] private RectTransform giftWindow;

        [SerializeField] private RectTransform container;
        [SerializeField] private GiftItem prefabItem;

        [Header("Null Message")]
        [SerializeField] private TextMeshProUGUI nullMessage;
        
        private CancellationTokenSource refreshCts;
        
        private void Awake()
        {
            closeThisButton.onClick.AddListener(ClosePanel);
            getAllButton.onClick.AddListener(OnGetAllButtonClicked);
        }
        private void OnEnable()
        {
            _ = RefreshPanelAsync();
        }
        
        public override void ClosePanel()
        {
            WindowAnimation.SwipeWindow(giftWindow);
            PageCameraSwiper.IsSwipeable = true;
        }

        public void SubscriptionEvent() => _ = RefreshPanelAsync();
        private void OnGetAllButtonClicked() => OnTakeAllAsync();


        private async Task RefreshPanelAsync()
        {
            refreshCts?.Cancel();
            refreshCts = new CancellationTokenSource();
            var token = refreshCts.Token;

            try
            {
                getAllButton.interactable = false;
                nullMessage.gameObject.SetActive(true);
                nullMessage.text = "받을 수 있는 상품이 없습니다.";

                foreach (Transform child in container)
                {
                    if (child != null)
                        Destroy(child.gameObject);
                }

                var requestDict = await FirebaseManager.GetGiftListAsync();
                token.ThrowIfCancellationRequested();

                foreach (var kvp in requestDict)
                {
                    token.ThrowIfCancellationRequested();

                    var item = Instantiate(prefabItem, container);
                    item.GetComponent<GiftItem>().Init(
                        kvp["fromNickname"].ToString(),
                        kvp["giftId"].ToString(),
                        kvp["giftCount"].ToString(),
                        false,
                        OnTakePresentAsync
                    );
                }

                if (requestDict.Count > 0)
                {
                    getAllButton.interactable = true;
                    nullMessage.gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.LogError($"RefreshPanelAsync 에러 발생: {ex}");
            }
        }

        private async void OnTakeAllAsync()
        {
            var message = await FirebaseManager.TakeAllGiftRequestAsync();
            MessageDialog.ShowMessage(
                "선물 모두 받기 성공",
                message,
                DialogMode.Confirm,
                null
            );
        }

        private async void OnTakePresentAsync(string itemType, bool allTake = false)
        {
            var result = await FirebaseManager.TakeGiftRequestAsync(itemType);
            bool success = (bool)result["success"];

            if (success && !allTake)
            {
                MessageDialog.ShowMessage("선물 받기 성공", "선물 받기를 성공했습니다.", DialogMode.Confirm, null);
            }
            else if (!success)
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("선물 받기 실패", reason, DialogMode.Confirm, null);
            }
            else
            {
                Debug.Log("선물 모두 받기 중 에러 발생");
            }
        }
    }
}
