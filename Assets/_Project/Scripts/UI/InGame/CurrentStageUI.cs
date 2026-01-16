using CocoDoogy.GameFlow.InGame;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    public class CurrentStageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentStageText;

        private void Awake()
        {
            currentStageText.text = $"{InGameManager.Stage.theme.ToName()} 테마 - {InGameManager.Stage.index}";
        }
    }
}
