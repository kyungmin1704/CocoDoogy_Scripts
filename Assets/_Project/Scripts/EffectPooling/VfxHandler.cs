using UnityEngine;
using System.Collections;
using Lean.Pool;
using UnityEngine.VFX;
using CocoDoogy.Tile;

/*  
    VfxHandler는 각 VFX 프리팹의 수명 관리용 스크립트입니다.
    내부 파티클/VFX를 자동 재생 및 반환합니다.
    Create()는 자기 자신의 프리팹을 풀에서 꺼내 실행합니다.
    각 VFX가 자기 생명주기를 스스로 관리하는 구조 입니다.
*/

namespace CocoDoogy.EffectPooling
{
    public class VfxHandler : MonoBehaviour
    {
        private ParticleSystem[] particleSystems;
        private VisualEffect[] visualEffects;
        private float maxLifetime;

        private Coroutine returnRoutine;

        private void Awake()
        {
            // 자식까지 전부 포함된 파티클 시스템 캐싱
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            visualEffects = GetComponentsInChildren<VisualEffect>(true);

            // Lifetime 계산
            CalculateMaxLifetime();
        }

        public static VfxHandler Create(VfxHandler prefab, Vector3 pos, Quaternion rot)
        {
            if (prefab == null)
            {
                Debug.LogError("[VfxHandler] 프리팹이 없습니다.");
                return null;
            }

            VfxHandler vfx = LeanPool.Spawn(prefab, pos, rot);

            vfx.Play();
            return vfx;
        }

        private void Play()
        {
            foreach (var ps in particleSystems)
                ps.Play(true);

            foreach (var vfx in visualEffects)
                vfx.Play();

            if (returnRoutine != null)
                StopCoroutine(returnRoutine);
            returnRoutine = StartCoroutine(ReturnAfterDelay(maxLifetime));
        }

        private IEnumerator ReturnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Release();
        }

        private void Release()
        {
            foreach (var ps in particleSystems)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            foreach (var vfx in visualEffects)
            {
                vfx.Stop();
                vfx.Reinit();
            }
            VfxManager.RemoveVfx(this);
            LeanPool.Despawn(gameObject, 0.5f);
        }

        private void CalculateMaxLifetime()
        {
            maxLifetime = 0;

            // 파티클 lifetime 계산
            foreach (var ps in particleSystems)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                if (duration > maxLifetime)
                    maxLifetime = duration;
            }

            // VFX lifetime 계산
            foreach (var vfx in visualEffects)
            {
                if (vfx.HasFloat("Duration"))
                {
                    float duration = vfx.GetFloat("Duration");
                    if (duration > maxLifetime)
                        maxLifetime = duration;
                }
            }

            // 계산 실패 시 기본값 3초
            if (maxLifetime <= 0)
            {
                maxLifetime = 3f;
            }
        }
    }
}