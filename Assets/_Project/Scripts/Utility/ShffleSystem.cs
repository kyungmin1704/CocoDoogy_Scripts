using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Utility
{
    public static class ShffleSystem
    {
        /// <summary>
        /// Fisher–Yates 알고리즘을 이용해 리스트를 섞는다.
        /// IList<T> 확장 메서드. List뿐만 아니라 배열에도 사용가능.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 1)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                int randIndex = Random.Range(i, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
        }
    }
}
