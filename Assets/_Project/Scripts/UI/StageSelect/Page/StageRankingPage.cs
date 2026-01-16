using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.StageSelect.Item;
using Lean.Pool;
using System;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Page
{
    public class StageRankingPage : StageInfoPage
    {
        public int CurrentStageStar { get; set; } = 0;
        [SerializeField] private RankItem prefab;
        [SerializeField] private RectTransform container;

        [SerializeField] private RankItem userRankItem;
        private RankData userRank = null;

        private void OnDisable()
        {
            userRankItem.Clear();
            userRank = null;
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                LeanPool.Despawn(container.GetChild(i).gameObject);
            }
        }

        protected override async void OnShowPage()
        {
            var ranking =
                await FirebaseManager.GetRanking((StageData.theme.ToIndex() + 1).Hex2(), StageData.index.Hex2());
            var sortedRanking = ranking.OrderBy(pair => pair.Value.rank).ToList();

            var userRecord = await FirebaseManager.GetRecord(StageData.theme.ToIndex() + 1, StageData.index);

            foreach (var kvp in sortedRanking)
            {
                RankData rank = kvp.Value;
                if (rank.nickname == DataManager.Instance.UserData.NickName)
                {
                    userRank = rank;
                }

                RankItem rankItem = LeanPool.Spawn(prefab, container);
                rankItem.Init(rank.rank.ToString(),
                    rank.nickname,
                    rank.refillPoints.ToString(),
                    rank.remainAP,
                    (float)rank.clearTime,
                    rank.replayId,
                    CurrentStageStar,
                    StageData);
            }

            if (userRecord is null)
            {
                userRankItem.Clear();
                return;
            }
            
            if (userRank is null)
            {
                userRankItem.Init("순위외", DataManager.Instance.UserData.NickName, userRecord.refillPoints.ToString(),
                userRecord.remainAP.ToString(), userRecord.clearTime, userRecord.replayId, CurrentStageStar, StageData);
            }
            else
            {
                userRankItem.Init(userRank.rank.ToString(), DataManager.Instance.UserData.NickName, userRank.refillPoints.ToString(),
                    userRank.remainAP, (float)userRank.clearTime, userRank.replayId, CurrentStageStar, StageData);
            }
        }
    }
}