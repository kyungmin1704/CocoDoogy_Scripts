using CocoDoogy.EmoteBillboard;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.UI.Popup;
using UnityEngine.SceneManagement;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// ActionPoint가 부족해서 이동 불가에 빠졌는지 체크
    /// </summary>
    public class ActionPointCheckPhase : IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            HexTile nextTile = HexTile.GetTile(PlayerHandler.GridPos);
            if (InGameManager.ActionPoints < nextTile.CurrentData.RealMoveCost)
            {
                if (InGameManager.RefillPoints < 1)
                {
                    ProcessDefeat();
                    return false;
                }
            }

            if (InGameManager.RefillPoints >= 0) return true;

            // TODO: 상징적인 패배를 넣어야 함.
            ProcessDefeat();
            return false;
        }

        private void ProcessDefeat()
        {
            // 슬픔 감정 트리거 (게임오버 확정)
            EmotionSystemHandler.TriggerGameDefeat();

            InGameManager.Timer.Pause();
            ItemHandler.UseItem(() =>
            {
                GameEndPopup.OpenPopup(true, 0);
                InGameManager.Timer.Stop();
            });
        }
    }
}