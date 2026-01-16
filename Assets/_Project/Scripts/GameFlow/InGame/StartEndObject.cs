using CocoDoogy.Data;
using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class StartEndObject: MonoBehaviour
    {
        [SerializeField] private bool isEnd = false;

        
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
            transform.position = (isEnd ? HexTileMap.EndPos : HexTileMap.StartPos).ToWorldPos();
        }
    }
}