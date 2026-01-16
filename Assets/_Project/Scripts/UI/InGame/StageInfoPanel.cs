using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    public class StageInfoPanel: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;


        void Awake()
        {
            OnMapDrawn(InGameManager.Stage);
            InGameManager.OnMapDrawn += OnMapDrawn;
        }
        void OnDestroy()
        {
            InGameManager.OnMapDrawn -= OnMapDrawn;
        }


        private void OnMapDrawn(StageData stage)
        {
            text.SetText(stage ? $"{stage.theme.ToName()} - {stage.index}" : "테스트맵");
        }
    }
}