using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.UI.Popup;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    /// <summary>
    /// InGame 내에서 초기화용 버튼<br/>
    /// 갖고 있는 남은 행동력을 모두 소진하고, 초기화 진행 및 초기화 카운팅
    /// </summary>
    public class ResetButton : MonoBehaviour
    {
        [SerializeField] private CommonButton button;


        void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        void OnEnable()
        {
            OnRefillCountChanged(InGameManager.RefillPoints);
            InGameManager.OnRefillCountChanged += OnRefillCountChanged;
        }
        void OnDisable()
        {
            InGameManager.OnRefillCountChanged -= OnRefillCountChanged;
        }


        private void OnButtonClicked()
        {
            MessageDialog.ShowMessage("원위치", "시작 지점으로 돌아가시겠습니까?", DialogMode.YesNo, OnMessageCallback);
        }
        private void OnMessageCallback(CallbackType type)
        {
            if (type != CallbackType.Yes) return;

            PlayerHandler.OnEventCallback?.Invoke(Vector2Int.zero, PlayerEventType.Refill);
            CommandManager.Refill();
        }

        private void OnRefillCountChanged(int refillPoints)
        {
            button.interactable = refillPoints > 0;
        }
    }
}