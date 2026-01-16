using CocoDoogy.Audio;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.InGame
{
    public class InGameItemButton : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image plusImage;
        public Button Button { get; private set; }
        public Image ButtonColor { get; private set; }

        public Image PlusImage
        {
            get => plusImage;
            set => plusImage = value;
        }
        
        /// <summary>
        /// 해당 버튼이 가지고 있는 ItemData를 InGameItemUI에서 넣어줌
        /// </summary>
        public ItemData ItemData { get; set; }

        /// <summary>
        /// 아이템 버튼을 눌렀을 때 작동하는 이벤트
        /// </summary>
        public Action<InGameItemButton, ItemData> OnClicked;
        
        /// <summary>
        /// 아이템을 InGame내에서 구매했는지 여부
        /// </summary>
        public bool IsPurchased { get; private set; } = false; 
        
        private void Awake()
        {
            if (!Button)
            {
                Button = GetComponent<Button>();
            }

            if (!ButtonColor)
            {
                ButtonColor = GetComponent<Image>();
            }
            
            Button.onClick.AddListener(() =>
            {
                OnClicked?.Invoke(this, ItemData);
                SfxManager.PlaySfx(SfxType.UI_ButtonUp1);
            });
        }

        /// <summary>
        /// 아이템 사용 메서드
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public void UseItem(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes) return;
            if (itemData.effect == ItemEffect.UndoTurn && !PlayerHandler.IsBehaviour)
            {
                MessageDialog.ShowMessage("아이템 사용 실패", "해당 아이템은 1턴 전으로 돌아가는 아이템입니다.\n 진행한 기록이 없으면 사용이 불가합니다.",
                    DialogMode.Confirm, null);
                return;
            }

            if (itemData.effect == ItemEffect.ConsumeAndRecoverMaxAP && InGameManager.ActionPoints <= 0)
            {
                MessageDialog.ShowMessage("아이템 사용 실패", "해당 아이템은 현재 행동력을 1 깍고 최대행동력을 1 늘리는 아이템입니다.\n 깍을 행동력이 없으면 사용이 불가합니다.",
                    DialogMode.Confirm, null);
                return;
            }
            switch (itemData.effect)
            {
                case ItemEffect.ConsumeAndRecoverMaxAP:
                    Debug.Log("행동력을 1 소모하고 최대 행동력을 1 증가시킵니다.");
                    CommandManager.MaxUp(itemData.effect);
                    SfxManager.PlaySfx(SfxType.Item_DogSleeping);
                    break;
                case ItemEffect.RecoverAP:
                    Debug.Log("행동력을 1 증가시킵니다.");
                    CommandManager.Recover(itemData.effect);
                    SfxManager.PlaySfx(SfxType.Item_Eating);
                    break;
                case ItemEffect.UndoTurn:
                    Debug.Log("1턴 전으로 돌아갑니다.");
                    CommandManager.Undo(itemData.effect);
                    SfxManager.PlaySfx(SfxType.Item_Undo);
                    break;
                case ItemEffect.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// 아이템 구매 메서드 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public async Task PurchaseAsync(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes || IsPurchased) return;
            try
            {
                var result = await FirebaseManager.PurchaseWithCashMoneyAsync(itemData.itemId, 1);

                bool success = result.ContainsKey("success") && (bool)result["success"];

                if (success)
                {
                    Debug.Log($"구매 성공: {itemData.itemName} ({1})");
                    DataManager.Instance.CurrentItem[itemData] += 1;
                    IsPurchased = true;
                    PlusImage.gameObject.SetActive(false);
                    SetColor(true);
                }
                else
                {
                    string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                    Debug.LogWarning($"구매 실패: {reason}");
                    MessageDialog.ShowMessage("구매실패", reason, DialogMode.Confirm, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"구매 실패: {e.Message}");
            }
        }

        public void SetColor(bool active)
        {
            float rgb = active ? 1f : 0.5f;
            Color targetColor = new(rgb, rgb, rgb);
            buttonImage.DOColor(targetColor, 0.2f);
            itemImage.DOColor(targetColor, 0.2f);
        }
    }
}