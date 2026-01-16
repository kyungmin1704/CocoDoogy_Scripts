using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace CocoDoogy.Utility
{
    public class Stopwatch
    {
        public float NowTime => time;
        public Action<float> OnTimeChanged = null;


        private float time = 0;
        private CancellationTokenSource token = null;


        /// <summary>
        /// 타이머를 시작
        /// </summary>
        public void Start()
        {
            if(token != null) return;
            _ = TimeAsync();
        }

        /// <summary>
        /// 타이머를 종료시키고, 축적된 시간을 0으로 초기화
        /// </summary>
        public void Stop()
        {
            Pause();
            Clear();
        }

        /// <summary>
        /// 축적된 시간을 0으로 초기화
        /// </summary>
        public void Clear()
        {
            time = 0;
        }
        /// <summary>
        /// 타이머 일시정지
        /// </summary>
        public void Pause()
        {
            token?.Cancel();
            token = null;
        }



        private async UniTask TimeAsync()
        {
            token = new();
            while(true)
            {
                await UniTask.Yield(token.Token);
                time += Time.deltaTime;
                OnTimeChanged?.Invoke(time);
            }
            token = null;
        }
    }
}