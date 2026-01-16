using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Audio
{
    [CreateAssetMenu(fileName = "SfxListData", menuName = "ScriptableObjects/SfxListData")]
    public class SfxListData : ScriptableObject
    {
        [Header("SFX List")]
        public List<SfxReference> sfxList = new();
        
        //일부 컴퓨터에서 오류 발생으로 주석 처리했습니다. 지우지 마세요
        
        // #region  SfxList Reset
        // //sfxList를 Reset 버튼 누름으로써 빠르게 초기화 하는 기능
        // [ContextMenu("Reset Sfx List")]
        // private void ResetSfxList()
        // {
        //     sfxList.Clear();
        //     int successCount = 0;
        //     int failCount = 0;
        //     var allEvents = EventManager.Events;
        //     
        //     foreach (SfxType sfxType in System.Enum.GetValues(typeof(SfxType)))
        //     {
        //         if (sfxType == SfxType.None) continue;
        //         string enumName = sfxType.ToString();
        //         
        //         string[] parts = enumName.Split('_');
        //         if (parts.Length != 2)
        //         {
        //             Debug.LogError($"SfxType {enumName}은 적합한 형식 아님! 이름 바꾸쇼");
        //             failCount++;
        //             continue;
        //         }
        //         
        //         string category = parts[0];
        //         string soundName = parts[1];
        //         
        //         //분리한 이름 기반으로 FMOD 이벤트 경로 생성
        //         string eventPath = $"event:/SFX/{category}/{soundName}";
        //         EventReference eventRef = EventReference.Find(eventPath);
        //         
        //         sfxList.Add(new SfxReference
        //         {
        //             type = sfxType,
        //             eventReference = eventRef
        //         });
        //         
        //         if (eventRef.IsNull)
        //         {
        //             Debug.LogWarning($"FMOD 이벤트 없음: {eventPath}");
        //             failCount++;
        //         }
        //         else
        //         {
        //             Debug.Log($"매칭 성공: {sfxType} -> {eventPath}");
        //             successCount++;
        //         }
        //     }
        //     
        //     Debug.Log($"=== 자동 매칭 완료 ===\n 성공: {successCount}개 | 실패: {failCount}개");
        // }
        // #endregion
    }
}
