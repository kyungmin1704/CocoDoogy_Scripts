using CocoDoogy.Data;
using Firebase.Functions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        /// <summary>
        /// 스테이지를 클리어하면 작동하는 메서드.
        /// </summary>
        /// <returns></returns>
        public static async Task<IDictionary<string, object>> ClearStageAsync(int theme, int level, int remainAP,
            int refillPoints, float clearTime, int star, string saveJson, Action openPopup)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                Dictionary<string, object> data = new()
                {
                    { "theme", theme.Hex2() },
                    { "level", level.Hex2() },
                    { "clearTime", clearTime },
                    { "remainAP", remainAP },
                    { "replayData", saveJson },
                    { "star", star },
                    { "refillPoints", refillPoints },
                };
                HttpsCallableResult result = await Instance.Functions.GetHttpsCallable("stageClear").CallAsync(data);

                string json = JsonConvert.SerializeObject(result.Data);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"스테이지 클리어 저장 실패: {e.Message}");
                throw;
            }
            finally
            {
                loading.Hide();
                openPopup?.Invoke();
            }
        }

        /// <summary>
        /// 클리어 한 스테이지 정보를 찾아 가장 최근에 클리어한 스테이지를 반환.
        /// </summary>
        /// <returns></returns>
        public static async Task<StageInfo> GetLastClearStage(string uid)
        {
            try
            {
                var snapshot = await Instance.Firestore
                    .Collection($"users/{uid}/stageInfo")
                    .GetSnapshotAsync();
                // 유저가 클리어한 정보를 내림차순으로 정리하여 가장 첫번째 정보를 반환 (가장 높은 스테이지 찾기)
                var lastDoc = snapshot.Documents
                    .Select(doc => new
                    {
                        Doc = doc,
                        IdValue = Convert.ToInt32(doc.Id, 16)
                    })
                    .OrderByDescending(x => x.IdValue)
                    .First().Doc;

                // StageInfo로 변환
                StageInfo stage = lastDoc.ConvertTo<StageInfo>();

                return stage;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<int> GetStageScore(int refillCount, int[] scores)
        {
            var loading = FirebaseLoading.ShowLoading();
            List<int> scoresList = scores.ToList();
            Dictionary<string, object> data = new() { { "refillCount", refillCount }, { "scores", scoresList }, };
            try
            {
                HttpsCallableResult result =
                    await Instance.Functions.GetHttpsCallable("stageClearScore").CallAsync(data);

                if (result.Data is IDictionary dict && dict.Contains("stars"))
                {
                    Debug.Log($"dict[\"star\"]: {Convert.ToInt32(dict["stars"])}");
                    return Convert.ToInt32(dict["stars"]);
                }

                Debug.Log("안됨");
                return 1;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firebase 호출 실패: {ex.Message}");
                return 1;
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<int> GetStar(int theme, int level)
        {
            string stageId = $"{theme.Hex2()}{level.Hex2()}";

            var docRef = Instance.Firestore
                .Document($"users/{Instance.Auth.CurrentUser.UserId}/stageInfo/{stageId}");

            var doc = await docRef.GetSnapshotAsync();

            if (doc.Exists && doc.TryGetValue("star", out int star))
            {
                return star;
            }

            return 0;
        }

        public static async Task<StageInfo> GetRecord(int theme, int level)
        {
            string stageId = $"{theme.Hex2()}{level.Hex2()}";
            
            var snapshot = await Instance.Firestore
                .Document($"users/{Instance.Auth.CurrentUser.UserId}/stageInfo/{stageId}")
                .GetSnapshotAsync();

            if (snapshot.Exists)
            {
                StageInfo stageInfo = snapshot.ConvertTo<StageInfo>();
                return stageInfo;
            }

            Debug.Log("해당 문서가 존재하지 않거나 찾지 못했습니다.");
            return null;
        }
    }
}