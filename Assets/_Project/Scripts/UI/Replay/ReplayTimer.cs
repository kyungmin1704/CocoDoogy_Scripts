using CocoDoogy.GameFlow.InGame;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.Replay
{
    public class ReplayTimer: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timer;

        public void SetTimer(double time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            timer.text = $"{minutes:00}:{seconds:00}";
        }
    }
}