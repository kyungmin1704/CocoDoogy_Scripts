using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Tutorial
{
    public static class TutorialLocker
    {
        public static bool CameraLock { get; set; } = false;

        public static List<Vector2Int> WhiteListPoses { get; } = new();


        public static bool CanPos(Vector2Int gridPos) =>
            WhiteListPoses.Count <= 0 || WhiteListPoses.Contains(gridPos);
    }
}