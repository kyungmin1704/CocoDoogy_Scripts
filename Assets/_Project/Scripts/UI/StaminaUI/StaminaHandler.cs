using UnityEngine;
using System;
using CocoDoogy.Core;

/*  
    StaminaHandler는 행동력을 관리하는 시스템 클래스입니다.
    행동력 증감 로직과 이벤트 발생을 담당합니다.
    StaminaUI에서 행동력 변경 이벤트를 구독하여 UI를 업데이트 합니다.
*/

namespace CocoDoogy.UI.StaminaUI
{
    public class StaminaHandler : Singleton<StaminaHandler>
    {
        [Header("행동력 설정")]
        [SerializeField] private int maxStamina = 5;
        [SerializeField] private int currentStamina = 5;

        // 행동력 변경 이벤트 (현재값, 최대값)
        public event Action<int, int> OnStaminaChanged;

        // 행동력 증가/감소 이벤트 (변경된 양)
        public event Action<int> OnStaminaGained;
        public event Action<int> OnStaminaLost;

        public int CurrentStamina => currentStamina;
        public int MaxStamina => maxStamina;

        protected override void Awake()
        {
            base.Awake();

            // 초기화 시 현재 행동력을 최대값으로 설정
            currentStamina = maxStamina;
        }

        private void Start()
        {
            // 초기 UI 업데이트
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        // 행동력 사용
        public bool UseStamina(int amount = 1)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("[StaminaHandler] 사용할 행동력은 0보다 커야 합니다.");
                return false;
            }

            if (currentStamina < amount)
            {
                // TODO : 행동력이 부족할 때 처리
                Debug.Log("[StaminaHandler] 행동력이 부족합니다.");
                return false;
            }

            currentStamina -= amount;
            OnStaminaLost?.Invoke(amount);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);

            Debug.Log($"[StaminaHandler] 행동력 사용: -{amount} (현재: {currentStamina}/{maxStamina})");
            return true;
        }

        // 행동력 회복
        public void GainStamina(int amount = 1)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("[StaminaHandler] 회복할 행동력은 0보다 커야 합니다.");
                return;
            }

            int previousStamina = currentStamina;
            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
            int actualGained = currentStamina - previousStamina;

            if (actualGained > 0)
            {
                OnStaminaGained?.Invoke(actualGained);
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                Debug.Log($"[StaminaHandler] 행동력 회복: +{actualGained} (현재: {currentStamina}/{maxStamina})");
            }
        }

        // 행동력 전체 회복
        public void RestoreAllStamina()
        {
            if (currentStamina < maxStamina)
            {
                int gained = maxStamina - currentStamina;
                currentStamina = maxStamina;
                OnStaminaGained?.Invoke(gained);
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                Debug.Log($"[StaminaHandler] 행동력 전체 회복 (현재: {currentStamina}/{maxStamina})");
            }
        }

        // 행동력 설정
        public void SetStamina(int value)
        {
            value = Mathf.Clamp(value, 0, maxStamina);

            if (currentStamina != value)
            {
                int difference = value - currentStamina;
                currentStamina = value;

                if (difference > 0)
                    OnStaminaGained?.Invoke(difference);
                else if (difference < 0)
                    OnStaminaLost?.Invoke(-difference);

                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                Debug.Log($"[StaminaHandler] 행동력 설정: {currentStamina}/{maxStamina}");
            }
        }

        // 최대 행동력 설정
        public void SetMaxStamina(int newMax, bool adjustCurrent = true)
        {
            if (newMax <= 0)
            {
                Debug.LogWarning("[StaminaHandler] 최대 행동력은 0보다 커야 합니다.");
                return;
            }

            maxStamina = newMax;

            if (adjustCurrent)
            {
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }

            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            Debug.Log($"[StaminaHandler] 최대 행동력 설정: {maxStamina}");
        }

        // 행동력 충분한지 확인
        public bool HasEnoughStamina(int required = 1)
        {
            return currentStamina >= required;
        }

        // 행동력 최대치인지 확인
        public bool IsFullStamina()
        {
            return currentStamina >= maxStamina;
        }

        // 행동력 0인지 확인
        public bool IsEmptyStamina()
        {
            return currentStamina <= 0;
        }
    }
}

