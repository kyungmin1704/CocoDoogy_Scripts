using UnityEngine;

/*  
    ActionPointTester는 행동력 시스템을 테스트하기 위한 스크립트입니다.
    키보드 입력으로 행동력을 증감시킬 수 있습니다.
*/

namespace CocoDoogy.UI.StaminaUI
{
    public class StaminaTester : MonoBehaviour
    {
        [Header("테스트 설정")]
        [SerializeField] private bool enableKeyboardTest = true;

        private StaminaHandler staminaSystem;

        private void Start()
        {
            staminaSystem = StaminaHandler.Instance;

            if (staminaSystem == null)
            {
                Debug.LogError("[StaminaTester] StaminaHandler을 찾을 수 없습니다!");
                return;
            }

            // 이벤트 구독 (로그 출력용)
            staminaSystem.OnStaminaGained += OnGained;
            staminaSystem.OnStaminaLost += OnLost;

            PrintHelp();
        }

        private void OnDestroy()
        {
            if (staminaSystem != null)
            {
                staminaSystem.OnStaminaGained -= OnGained;
                staminaSystem.OnStaminaLost -= OnLost;
            }
        }

        private void Update()
        {
            if (!enableKeyboardTest) return;

            // Space: 행동력 1 사용
            if (Input.GetKeyDown(KeyCode.Space))
            {
                staminaSystem.UseStamina(1);
            }

            // R: 행동력 1 회복
            if (Input.GetKeyDown(KeyCode.R))
            {
                staminaSystem.GainStamina(1);
            }

            // F: 행동력 전체 회복
            if (Input.GetKeyDown(KeyCode.F))
            {
                staminaSystem.RestoreAllStamina();
            }

            // E: 행동력 전체 소진
            if (Input.GetKeyDown(KeyCode.E))
            {
                staminaSystem.SetStamina(0);
            }

            // 1~9: 행동력을 특정 값으로 설정
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    staminaSystem.SetStamina(i);
                }
            }

            // M: 최대 행동력 증가
            if (Input.GetKeyDown(KeyCode.M))
            {
                staminaSystem.SetMaxStamina(staminaSystem.MaxStamina + 1, true);
            }

            // N: 최대 행동력 감소
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (staminaSystem.MaxStamina > 1)
                {
                    staminaSystem.SetMaxStamina(staminaSystem.MaxStamina - 1, true);
                }
            }

            // H: 도움말 출력
            if (Input.GetKeyDown(KeyCode.H))
            {
                PrintHelp();
            }
        }

        private void OnGained(int amount)
        {
            Debug.Log($"<color=green>[StaminaTester] 행동력 획득! +{amount}</color>");
        }

        private void OnLost(int amount)
        {
            Debug.Log($"<color=red>[StaminaTester] 행동력 손실! -{amount}</color>");
        }

        private void PrintHelp()
        {
            Debug.Log("========== 행동력 테스트 컨트롤 ==========\n" +
                      "Space: 행동력 1 사용\n" +
                      "R: 행동력 1 회복\n" +
                      "F: 행동력 전체 회복\n" +
                      "E: 행동력 전체 소진\n" +
                      "1~9: 행동력을 특정 값으로 설정\n" +
                      "M: 최대 행동력 증가\n" +
                      "N: 최대 행동력 감소\n" +
                      "H: 도움말 출력\n" +
                      "=========================================");
        }

        // 인스펙터에서 버튼으로 테스트할 수 있는 메서드들
        [ContextMenu("행동력 사용")]
        public void TestUseStamina()
        {
            if (staminaSystem != null)
                staminaSystem.UseStamina(1);
        }

        [ContextMenu("행동력 회복")]
        public void TestGainStamina()
        {
            if (staminaSystem != null)
                staminaSystem.GainStamina(1);
        }

        [ContextMenu("행동력 전체 회복")]
        public void TestRestoreAllStamina()
        {
            if (staminaSystem != null)
                staminaSystem.RestoreAllStamina();
        }

        [ContextMenu("행동력 전체 소진")]
        public void TestDepleteAllStamina()
        {
            if (staminaSystem != null)
                staminaSystem.SetStamina(0);
        }
    }
}

