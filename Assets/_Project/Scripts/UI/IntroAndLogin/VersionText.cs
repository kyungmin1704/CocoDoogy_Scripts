using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.IntroAndLogin
{
    /// <summary>
    /// Intro Scene에서 Version을 띄우는 용도
    /// </summary>
    [ExecuteAlways]
    public class VersionText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;


        void Update()
        {
            RectTransform rect = transform as RectTransform;
            rect.anchoredPosition = Vector2.one * 25f;

            text.text = $"v{Application.version}";
        }
    }
}