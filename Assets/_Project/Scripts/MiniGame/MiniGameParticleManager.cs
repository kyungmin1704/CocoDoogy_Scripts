using CocoDoogy.Core;
using Coffee.UIExtensions;
using Lean.Pool;
using System.Collections;
using UnityEngine;

namespace CocoDoogy.MiniGame
{
    public class MiniGameParticleManager : Singleton<MiniGameParticleManager>
    {
        [SerializeField] private RectTransform canvas;
        [SerializeField] private UIParticle coatParticlePrefab;
        [SerializeField] private UIParticle wateringParticlePrefab;
        [SerializeField] private UIParticle diggingParticlePrefab;
        public void PlayParticle(UIParticle particlePrefab, Vector3 position, Transform parent)
        {
            UIParticle p = LeanPool.Spawn(particlePrefab, position, Quaternion.identity, parent);
            p.Play();
            StartCoroutine(RetrunAfter(p, p.timeScaleMultiplier));
        }

        public void ParticleWatering(Transform parent)
        {
            Instantiate(wateringParticlePrefab, parent);
        }

        public void ParticleDigging(Vector3 position, Transform parent) => PlayParticle(diggingParticlePrefab, position, parent);

        public void ParticleCoat(Vector3 position, Transform parent) => PlayParticle(coatParticlePrefab, position, parent);


        IEnumerator RetrunAfter(UIParticle p, float time)
        {
            yield return new WaitForSeconds(time);
            LeanPool.Despawn(p);
        }
    }
}
