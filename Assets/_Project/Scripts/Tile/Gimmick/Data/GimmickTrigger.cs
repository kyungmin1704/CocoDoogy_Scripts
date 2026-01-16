using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Tile.Gimmick.Data
{
    [System.Serializable]
    public class GimmickTrigger
    {
        public Vector2Int GridPos = Vector2Int.zero;
        public bool IsReversed = false;
        
        
        /// <summary>
        /// 해당 트리거가 동작상태인지 여부
        /// </summary>
        public bool IsTriggered { get; set; } = false;
    }
}