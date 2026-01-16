using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Tile;
using UnityEngine;
using TMPro;
using System;

namespace CocoDoogy.UI.InGame
{
    public class RefillPointUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        void OnEnable()
        {
            InGameManager.OnRefillCountChanged += OnRefillCountChanged;
        }
        void OnDisable()
        {
            InGameManager.OnRefillCountChanged -= OnRefillCountChanged;
        }


        private void OnRefillCountChanged(int point)
        {
            text.SetText($"{Math.Clamp(point, 0, int.MaxValue)} / {HexTileMap.RefillPoint}");
        }
    }
}