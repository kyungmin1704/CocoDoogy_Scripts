using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class SearchWindowPopup : MonoBehaviour
    {
        /// <summary>
        /// 친구 요청을 할 대상의 닉네임을 입력받기 위한 InputField
        /// </summary>
        [SerializeField] private TMP_InputField searchInputField;

        /// <summary>
        /// InputField에서 입력된 값을 가져오기 위한 프로퍼티
        /// </summary>
        public string InputNickname => searchInputField.text;

        /// <summary>
        /// 해당 패널이 켜질 때 InputField에 적힌 내용 초기화
        /// </summary>
        private void OnEnable()
        {
            searchInputField.text = string.Empty;
        }
    }
}