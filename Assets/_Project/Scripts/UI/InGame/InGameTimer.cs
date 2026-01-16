using CocoDoogy.GameFlow.InGame;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    public class InGameTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;


        void Awake()
        {
            InGameManager.Timer.OnTimeChanged += OnTimeChanged;
        }
        void OnDestroy()
        {
            InGameManager.Timer.OnTimeChanged -= OnTimeChanged;
        }


        private void OnTimeChanged(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
