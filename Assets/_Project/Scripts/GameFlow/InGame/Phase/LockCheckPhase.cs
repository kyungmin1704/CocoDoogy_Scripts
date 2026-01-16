using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 더 움직일 수 없는 상태인지 체크
    /// </summary>
    public class LockCheckPhase: IPhase, IClearable
    {
        private CancellationTokenSource token = null;


        public void OnClear()
        {
            token?.Cancel();
            token = null;
        }

        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (!tile.IsPlaceable) // 타일이 이동불가 상태인지
            {
                _ = RefillAsync();
                return false;
            }
            
            return true;
        }


        private async UniTask RefillAsync()
        {
            token = new();
            PlayerHandler.Locked = true;
            await UniTask.WaitForSeconds(2f, cancellationToken: token.Token);
            PlayerHandler.Locked = false;

            CommandManager.Refill();
            token = null;
        }
    }
}