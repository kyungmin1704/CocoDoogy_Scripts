using CocoDoogy.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CocoDoogy.EffectPooling;
using Lean.Pool;

namespace CocoDoogy.Tile
{
    public class VfxManager : Singleton<VfxManager>
    {
        [SerializeField] private List<VfxEntry> vfxEntries = new(); // 딕셔너리에 등록할 프리팹과 enum 을 저장하는 리스트

        private readonly Dictionary<VfxType, VfxHandler> vfxDictionary = new();
        private readonly HashSet<VfxHandler> activeVfxs = new();

        protected override void Awake()
        {
            base.Awake();

            // VFX 프리팹 Dictionary 구성
            foreach (var entry in vfxEntries)
            {
                if (!vfxDictionary.TryAdd(entry.type, entry.prefab))
                    Debug.LogError($"[VfxManager] {entry.type} 프리팹이 중복 등록되었습니다.");
            }
        }

        /// <summary>
        /// VfxType에 해당하는 프리팹 반환
        /// </summary>
        public static VfxHandler GetVfx(VfxType type) => Instance?.vfxDictionary[type];

        /// <summary>
        /// 타입에 따라 프리팹을 생성하고 등록
        /// </summary>
        public static void CreateVfx(VfxType type, Vector3 position, Quaternion rotation)
        {
            VfxHandler prefab = GetVfx(type);
            if (prefab == null) return;

            VfxHandler vfx = VfxHandler.Create(prefab, position, rotation);
            AddVfx(vfx);
        }

        /// <summary>
        /// 활성 VFX 등록
        /// </summary>
        public static void AddVfx(VfxHandler vfx)
        {
            if (Instance == null || vfx == null) return;
            Instance.activeVfxs.Add(vfx);
        }

        /// <summary>
        /// 활성 VFX 제거
        /// </summary>
        public static void RemoveVfx(VfxHandler vfx)
        {
            if (Instance == null) return;
            Instance.activeVfxs.Remove(vfx);
        }

        /// <summary>
        /// 모든 활성 VFX 즉시 정리
        /// </summary>
        public static void ClearAllVfx()
        {
            if (Instance == null) return;

            foreach (var vfx in Instance.activeVfxs.ToArray())
            {
                if (vfx != null)
                    LeanPool.Despawn(vfx.gameObject);
            }
            Instance.activeVfxs.Clear();
        }

        /// <summary>
        /// 현재 활성 VFX 개수
        /// </summary>
        public static int ActiveVfxCount => Instance?.activeVfxs.Count ?? 0;
    }
}