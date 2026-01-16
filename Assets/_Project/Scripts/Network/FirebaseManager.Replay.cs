using CocoDoogy.Data;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        /// <summary>
        /// 스테이지의 랭킹을 가져오는 메서드
        /// </summary>
        /// <param name="theme">선택한 스테이지의 테마</param>
        /// <param name="level">선택한 스테이지의 레벨</param>
        /// <returns></returns>
        public static async Task<IDictionary<string, RankData>> GetRanking(string theme, string level)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                string stageInfo = $"{theme}{level}";
                var rankDoc = Instance.Firestore.Collection("rankings").Document(stageInfo);
                
                DocumentSnapshot rankSnapshot = await rankDoc.GetSnapshotAsync();
                
                if (!rankSnapshot.Exists)
                {
                    Debug.Log("스냅샷 없음");
                    return new Dictionary<string, RankData>();
                }

                var data = rankSnapshot.ToDictionary();
                var result = new Dictionary<string, RankData>();

                foreach (var pair in data)
                {
                    // pair.Value는 하위 Dictionary<string, object>임
                    if (pair.Value is Dictionary<string, object> rankMap)
                    {
                        RankData rankData = new()
                        {
                            rank = Convert.ToInt32(rankMap["rank"]),
                            clearTime = Convert.ToDouble(rankMap["clearTime"]),
                            remainAP = rankMap["remainAP"].ToString(),
                            replayId = rankMap["replayId"].ToString(),
                            refillPoints = Convert.ToInt32(rankMap["refillPoints"]),
                            nickname = rankMap["nickname"].ToString(),
                        };

                        result[pair.Key] = rankData;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<string> GetReplayData(string replayId)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                var replayDoc = Instance.Firestore.Collection("replays").Document(replayId);
                DocumentSnapshot replaySnapshot = await replayDoc.GetSnapshotAsync();
                
                if (!replaySnapshot.Exists)
                {
                    Debug.Log("스냅샷 없음");
                    return string.Empty;
                }
                string replayJson = replaySnapshot.GetValue<string>("replayData");

                return replayJson;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return string.Empty;
            }
            finally
            {
                loading.Hide();
            }
        }
    }
}