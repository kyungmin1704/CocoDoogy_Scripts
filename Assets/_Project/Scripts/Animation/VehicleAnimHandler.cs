using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace CocoDoogy.Animation
{
    public class VehicleAnimHandler : MonoBehaviour
    {
        private static readonly int VehicleType = Animator.StringToHash("VehicleType");
        private static readonly int Transport = Animator.StringToHash("Transport");
        
        private Animator anim;

        public static VehicleAnimHandler _instance;


        private void Awake()
        {
         
            anim = GetComponent<Animator>();
            _instance = this;
            gameObject.SetActive(false);
        }

        public static void SetActive()
        {
            _instance.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            PlayVehicleAnim(Animation.VehicleType.Ship, null).Forget();
        }

        /// <summary>
        /// 탈것으로 이동시 애니메이션
        /// UniTask로 애니메이션 시간만큼 딜레이를 준뒤에 Idle상태로 돌아감
        /// </summary>
        /// <param name="vehicleType">탈것의 타입에 따라 다른 애니메이션을 실행가능 애니메이션 클립으로 필요한 스프라이트 변경 해야함</param>
        /// <param name="onComplete">애니메이션 실행후 메서드</param>
        public async UniTask PlayVehicleAnim(VehicleType vehicleType, Action onComplete)
        {
            anim.SetInteger(VehicleType, (int)vehicleType);
            anim.SetTrigger(Transport);

            await UniTask.Yield();
            
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float animLength = stateInfo.length;
            
            await UniTask.Delay(TimeSpan.FromSeconds(animLength));
            
            VehicleToIdle();
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 이동후 탈것 애니메이션 초기화
        /// </summary>
        public void VehicleToIdle()
        {
            gameObject.SetActive(false);
            anim.SetInteger(VehicleType, 0);
            anim.SetTrigger(Transport);
        }
        
        
    }
}