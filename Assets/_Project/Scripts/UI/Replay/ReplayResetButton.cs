using CocoDoogy.GameFlow.InGame;
using System;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.Replay
{
    public class ReplayResetButton : MonoBehaviour
    {
        [SerializeField] private CommonButton replayResetButton;

        void OnEnable()
        {
            OnRefillCountChanged(InGameManager.RefillPoints);
            InGameManager.OnRefillCountChanged += OnRefillCountChanged;
        }
        void OnDisable()
        {
            InGameManager.OnRefillCountChanged -= OnRefillCountChanged;
        }
        
        private void OnRefillCountChanged(int refillPoints)
        {
            replayResetButton.interactable = refillPoints > 0;
        }
    }
}