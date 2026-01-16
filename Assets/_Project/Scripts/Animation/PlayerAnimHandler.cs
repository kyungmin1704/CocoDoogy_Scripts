using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace CocoDoogy.Animation
{
    public class PlayerAnimHandler : MonoBehaviour
    {
        private const float IDLE_TO_SIT_TIME = 5f;
        
        
        private static readonly int ActionType = Animator.StringToHash("ActionType");
        private static readonly int Changed = Animator.StringToHash("Changed");


        [SerializeField] private Animator anim;


        private CancellationTokenSource token = null;


        void OnDestroy()
        {
            token?.Cancel();
            token = null;
        }

        
        /// <summary>
        /// 애니메이터 전환
        /// </summary>
        /// <param name="type">전환할 애니메이션</param>
        public void ChangeAnim(AnimType type)
        {
            token?.Cancel();
            token = null;
            
            anim.SetInteger(ActionType, (int)type);
            anim.SetTrigger(Changed);

            if (type == AnimType.Idle)
            {
                _ = CheckIdleTimeAsync(IDLE_TO_SIT_TIME);
            }
        }

        /// <summary>
        /// Idle이 일정시간 이상 지속되면 SitDown처리 
        /// </summary>
        /// <param name="time">일정시간</param>
        private async UniTask CheckIdleTimeAsync(float time)
        {
            token = new();

            await UniTask.WaitForSeconds(time, cancellationToken: token.Token);
            ChangeAnim(AnimType.Sit);

            token = null;
        }
    }
}
