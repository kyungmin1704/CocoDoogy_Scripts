using TMPro;
using UnityEngine;

namespace CocoDoogy.Data
{
    public class UIDataHandler : MonoBehaviour
    {
        //부피가 커지면 이벤트 버스 방식으로 리펙토링 고려 (후순위)
        [SerializeField] private TextMeshProUGUI item001;
        [SerializeField] private TextMeshProUGUI item002;
        [SerializeField] private TextMeshProUGUI item003;
        
        private void Start()
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnPrivateUserDataLoaded += UpdateUI;

                if (DataManager.Instance.UserData != null)
                {
                    UpdateUI();
                }
            }
            else
            {
                Debug.LogError("DataManager.Instance가 null입니다!");
            }
            
        }

        private void OnDisable()
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnPrivateUserDataLoaded -= UpdateUI;
            }
        }
        
        private void UpdateUI()
        {
            print("UI 업데이트 시작");
            var userData = DataManager.Instance.UserData;
            
            if (userData == null) return;

            if (item001 != null)
            {
                if (userData.ItemDic.TryGetValue("item001", out object value1))
                {
                    item001.text = value1.ToString();
                }
                else
                {
                    item001.text = "0";
                }
            }

            if (item002 != null)
            {
                if (userData.ItemDic.TryGetValue("item002", out object value2))
                {
                    item002.text = value2.ToString();
                }
                else
                {
                    item002.text = "0";
                }
            }

            if (item003 != null)
            {
                if (userData.ItemDic.TryGetValue("item003", out object value3))
                {
                    item003.text = value3.ToString();
                }
                else
                {
                    item003.text = "0";
                }
            }
            print("UI 업데이트 종료");
        }
    }
}
