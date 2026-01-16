using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/*  
    ActionPointUI는 행동력을 화면에 표시하는 UI 핸들러입니다.
    ActionPointSystem에서 행동력 변경 이벤트를 구독하여 UI를 업데이트 합니다.
*/

namespace CocoDoogy.UI.StaminaUI
{
    public class StaminaUI : MonoBehaviour
    {
        [Header("UI 설정")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private StaminaSlot slotPrefab;

        [Header("배치 설정")]
        [SerializeField] private UIPosition position = UIPosition.BottomLeft;

        private List<StaminaSlot> slots = new List<StaminaSlot>();
        private StaminaHandler staminaSystem;

        public enum UIPosition
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private void Awake()
        {
            if (slotContainer == null)
                slotContainer = transform;
        }

        private void Start()
        {
            staminaSystem = StaminaHandler.Instance;

            if (staminaSystem == null)
            {
                Debug.LogError("[StaminaUI] StaminaHandler을 찾을 수 없습니다!");
                return;
            }

            // 이벤트 구독
            staminaSystem.OnStaminaChanged += UpdateUI;

            // 초기 UI 생성
            InitializeSlots();

            // 위치 설정
            SetPosition();
        }

        private void OnDestroy()
        {
            if (staminaSystem != null)
            {
                staminaSystem.OnStaminaChanged -= UpdateUI;
            }
        }


        private void InitializeSlots()
        {
            if (slotPrefab == null)
            {
                Debug.LogError("[StaminaUI] 슬롯 프리팹이 설정되지 않았습니다!");
                return;
            }

            // 기존 슬롯 제거
            ClearSlots();

            int maxPoints = staminaSystem.MaxStamina;

            // 슬롯 생성
            for (int i = 0; i < maxPoints; i++)
            {
                StaminaSlot slot = Instantiate(slotPrefab, slotContainer);
                slots.Add(slot);
            }

            // 초기 상태 설정 (애니메이션 없이)
            UpdateUI(staminaSystem.CurrentStamina, maxPoints);
        }

        private void UpdateUI(int currentStamina, int maxStamina)
        {
            // 슬롯 개수가 변경된 경우 재생성
            if (slots.Count != maxStamina)
            {
                InitializeSlots();
                return;
            }

            // 각 슬롯의 활성화 상태 업데이트
            for (int i = 0; i < slots.Count; i++)
            {
                if (i < currentStamina)
                {
                    slots[i].Activate();
                }
                else
                {
                    slots[i].Deactivate();
                }
            }
        }

        // UI 위치 설정
        private void SetPosition()
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null) return;

            switch (position)
            {
                case UIPosition.TopLeft:
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(0, 1);
                    rt.pivot = new Vector2(0, 1);
                    rt.anchoredPosition = new Vector2(20, -20);
                    break;

                case UIPosition.TopRight:
                    rt.anchorMin = new Vector2(1, 1);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.pivot = new Vector2(1, 1);
                    rt.anchoredPosition = new Vector2(-20, -20);
                    break;

                case UIPosition.BottomLeft:
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(0, 0);
                    rt.pivot = new Vector2(0, 0);
                    rt.anchoredPosition = new Vector2(20, 20);
                    break;

                case UIPosition.BottomRight:
                    rt.anchorMin = new Vector2(1, 0);
                    rt.anchorMax = new Vector2(1, 0);
                    rt.pivot = new Vector2(1, 0);
                    rt.anchoredPosition = new Vector2(-20, 20);
                    break;
            }
        }

        // 모든 슬롯 제거
        private void ClearSlots()
        {
            foreach (var slot in slots)
            {
                if (slot != null)
                    Destroy(slot.gameObject);
            }
            slots.Clear();
        }
    }
}



