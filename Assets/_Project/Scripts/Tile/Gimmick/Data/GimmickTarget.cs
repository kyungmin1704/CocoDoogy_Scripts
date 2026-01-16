using UnityEngine;

namespace CocoDoogy.Tile.Gimmick.Data
{
    [System.Serializable]
    public class GimmickTarget
    {
        public Vector2Int GridPos = Vector2Int.zero;
        public HexDirection Direction = HexDirection.Center;
    }
}