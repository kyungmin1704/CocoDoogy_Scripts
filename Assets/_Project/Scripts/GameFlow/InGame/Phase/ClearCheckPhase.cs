using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Network;
using CocoDoogy.Tile;
using CocoDoogy.UI.Popup;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 클리어 여부 확인
    /// </summary>
    public class ClearCheckPhase : IPhase
    {
        private int star = 3;
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            if (PlayerHandler.GridPos == HexTileMap.EndPos)
            {
                
                StageClearProcess();
                
                return false;
            }
            return true;
        }

        private async void StageClearProcess()
        {
            float time = InGameManager.Timer.NowTime;
            int remainAp = InGameManager.UseActionPoints;
            int refillCount = InGameManager.UseRefillCounts;
            string saveJson = CommandManager.Save();
            
            Debug.Log($"remainAp: {remainAp}, refillCount: {refillCount}");
            
            star = await FirebaseManager.GetStageScore(refillCount, InGameManager.Stage.starThresholds);
            // 여기서 Timer.Stop을 하면 Popup에 0초로 기록됨. 그래서 일단 시간을 멈추고
            InGameManager.Timer.Pause();

            ItemHandler.UseItem();
            
            if(!PlayerHandler.IsReplay)
            // 이 부분에서 popup이 열리고나서 시간이 초기화 되게 
            {
                _ = FirebaseManager.ClearStageAsync(InGameManager.Stage.theme.ToIndex() + 1,
                    InGameManager.Stage.index, remainAp, refillCount, time, star, saveJson,
                    () =>
                    {
                        GameEndPopup.OpenPopup(false, star);
                        InGameManager.Timer.Stop();
                    });
            }
            else
            {
                GameEndPopup.OpenPopup(false, star);
            }
        }
    }
}