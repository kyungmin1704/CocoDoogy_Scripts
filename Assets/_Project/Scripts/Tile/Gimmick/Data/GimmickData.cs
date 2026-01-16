using System.Linq;
using UnityEngine;

namespace CocoDoogy.Tile.Gimmick.Data
{
    /// <summary>
    /// 기믹에 대한 데이터를 구조화하는 용도의 class
    /// </summary>
    [System.Serializable]
    public class GimmickData
    {
        public GimmickType Type = GimmickType.None;

        public GimmickTarget Target = new();
        public GimmickEffect Effect = new();
        public System.Collections.Generic.List<GimmickTrigger> Triggers = new();
        
        
        /// <summary>
        /// 해당 기믹이 동작상태인지 여부
        /// </summary>
        public bool IsOn { get; set; } = false;


        public GimmickTrigger GetTrigger(Vector2Int gridPos)
        {
            return Triggers.FirstOrDefault(trigger => trigger.GridPos == gridPos);
        }
        public bool ContainsTrigger(Vector2Int gridPos)
        {
            return Triggers.Any(trigger => trigger.GridPos == gridPos);
        }
    }
}