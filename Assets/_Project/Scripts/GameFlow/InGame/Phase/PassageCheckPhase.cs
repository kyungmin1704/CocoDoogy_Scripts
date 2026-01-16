using CocoDoogy.GameFlow.InGame.Phase.Passage;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 현재 소모된 ActionPoint로 처리해야 할 동작을 판단
    /// </summary>
    public class PassageCheckPhase: IPhase
    {
        private int lastWorkActionPoints = -1;
        private HashSet<PassageBase> executed = new();
        

        public void OnClear()
        {
            lastWorkActionPoints = -1;
            executed.Clear();
        }
        
        
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            // 바로 전에 동작했었다면, 무시
            if (lastWorkActionPoints == InGameManager.ConsumedActionPoints) return true;
            lastWorkActionPoints = InGameManager.ConsumedActionPoints;
            
            int min = InGameManager.ConsumedActionPoints - InGameManager.LastConsumeActionPoints + 1;
            int max = InGameManager.ConsumedActionPoints;

            executed.Clear();
            for(int i = 0;i < InGameManager.Passages.Count;i++)
            {
                PassageBase passage = InGameManager.Passages[i];
                if (executed.Contains(passage)) continue;
                if (!passage.ActionPoints.IsBetween(min, max)) continue;

                executed.Add(passage);
                passage.Execute();

                if (InGameManager.Passages.Count < i && InGameManager.Passages[i] == passage) continue;
                i = -1;
            }
            
            return true;
        }
    }
}