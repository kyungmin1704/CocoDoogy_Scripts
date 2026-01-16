using UnityEngine;
using CocoDoogy.EffectPooling;
using CocoDoogy.Tile;

namespace CocoDoogy.EffectPooling
{
    /// <summary>
    /// VFX 테스트 스크립트
    /// 1: CreateVfx - 자동 생성 및 등록 (간편)
    /// C: ClearAllVfx - 모든 VFX 정리
    /// </summary>
    public class VfxTest : MonoBehaviour
    {
        [SerializeField] private VfxType vfxType = VfxType.None; // 테스트할 VFX 타입

        void Update()
        {
            // CreateVfx - 자동 생성 및 등록
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                VfxManager.CreateVfx(vfxType, transform.position, transform.rotation);
                Debug.Log($"[CreateVfx] {vfxType} 생성 | 활성 VFX: {VfxManager.ActiveVfxCount}");
            }

            // 모든 VFX 정리
            if (Input.GetKeyDown(KeyCode.C))
            {
                int count = VfxManager.ActiveVfxCount;
                VfxManager.ClearAllVfx();
                Debug.Log($"[ClearAll] {count}개 VFX 정리 완료");
            }
        }
    }
}
