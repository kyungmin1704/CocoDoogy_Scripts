using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.Network;
using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageListPage: MonoBehaviour
    {
        public const int LIST_SIZE = 100;
        [SerializeField] private StageSelectButton stageButtonPrefab;
        [SerializeField] private Transform stageGroup;

        private CancellationTokenSource drawCts;
        
        private Stack<StageSelectButton> spawnedButtons = new();


        void OnEnable()
        {
            stageGroup.position = new Vector2(0, stageGroup.position.y);
        }

        public async void DrawButtons(Theme theme, int start)
        {
            drawCts?.Cancel();
            drawCts = new CancellationTokenSource();
            var token = drawCts.Token;
            try
            {
                while (spawnedButtons.Count > 0)
                {
                    LeanPool.Despawn(spawnedButtons.Pop());
                }
            
                foreach (StageData data in DataManager.GetStageData(theme))
                {
                    if (data.index < start) continue;
                    
                    token.ThrowIfCancellationRequested();
                    
                    int star = await FirebaseManager.GetStar(data.theme.ToIndex() + 1, data.index);
                    
                    token.ThrowIfCancellationRequested();
                    
                    StageSelectButton stageButton = LeanPool.Spawn(stageButtonPrefab, stageGroup);
                    stageButton.Init(data, star, StageSelectManager.ShowReadyView);
                
                    spawnedButtons.Push(stageButton);
                    if (spawnedButtons.Count >= LIST_SIZE) break;
                }
            }
            catch (OperationCanceledException)
            {
                // 정상 취소, 로그 남기지 않기
                // Debug.Log("DrawButtons canceled"); // 필요하면 이렇게
            }
            catch (Exception ex)
            {
                // 진짜 오류만 찍기
                Debug.LogError(ex);
            }
        }
    }
}