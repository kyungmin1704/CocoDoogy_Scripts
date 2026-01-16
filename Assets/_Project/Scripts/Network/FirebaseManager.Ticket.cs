using CocoDoogy.Network.Ticket;
using Firebase.Functions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        public static Coroutine TicketCoroutine;
        
        public const int MaxRegenTicket = 10;
        private const long RechargeIntervalMs = 30 * 60 * 1000; // TODO: 지금은 1분 주기로 실행되게 되어있는데 나중에 10분 or 30분 주기로 변경 예정
        private int TotalTicket => CurrentTicket + BonusTicket;

        private int CurrentTicket { get; set; }
        private int BonusTicket { get; set; }
        private long? LastTicketTimestamp { get; set; } = 0;
        private TimeSpan TimeUntilNextTicket { get; set; } = TimeSpan.Zero;
        
        private long serverTimeOffset = 0;
        
        /// <summary>
        /// 티켓을 충전하거나 사용하면 그걸 클라이언트에 반영하는 메서드
        /// </summary>
        /// <param name="obj">Firebase Functions에서 반환된 값</param>
        private void UpdateTicketState(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            TicketResponse response = JsonConvert.DeserializeObject<TicketResponse>(json);

            if (!response.Success)
            {
                Debug.LogError($"티켓 작업 실패: {response.Reason}");
                return;
            }

            CurrentTicket = response.CurrentTicket;
            BonusTicket = response.BonusTicket;
            if (response.ServerTime is not null)
            { // 서버와 클라이언트 간의 시간차이 계산
                serverTimeOffset = Convert.ToInt64(response.ServerTime) - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            LastTicketTimestamp = response.LastTicketTime != null ? Convert.ToInt64(response.LastTicketTime) : null;
            
            // TODO : 로그 확인용이므로 나중에 삭제
            Debug.Log(response.Added > 0
                ? $"티켓 {response.Added}개 충전됨. 총 {TotalTicket}개 ({CurrentTicket} + {BonusTicket})"
                : $"티켓 상태 갱신됨.총 {TotalTicket}개 ({CurrentTicket} + {BonusTicket})");
        }

        /// <summary>
        /// 티켓을 충전하는 메서드
        /// </summary>
        public async Task RechargeTicketAsync()
        {
            try
            {
                var result = await Functions.GetHttpsCallable("rechargeTicket").CallAsync();
                UpdateTicketState(result.Data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase Function 호출 실패: {e.Message}");
            }
        }

        /// <summary>
        /// 티켓을 사용하는 메서드
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UseTicketAsync()
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                HttpsCallableResult result = await Instance.Functions.GetHttpsCallable("consumeTicket").CallAsync();
                string json = JsonConvert.SerializeObject(result.Data);
                TicketResponse response = JsonConvert.DeserializeObject<TicketResponse>(json);

                if (response.Success) Instance.UpdateTicketState(result.Data);

                return response.Success;
            }
            catch (Exception e)
            {
                Debug.LogError("티켓 사용 함수 호출 실패: " + e.Message);
                return false;
            }
            finally
            {
                loading.Hide();
            }
        }
        
        /// <summary>
        /// 로그인에 성공하면 일정 시간마다 티켓을 충전하는 코루틴을 실행 <br/>
        /// 로그아웃을 하면 코루틴을 멈춤.
        /// </summary>
        public void AuthStateChanged()
        {
            if (Auth.CurrentUser != null)
            {
                Debug.Log("로그인됨: " + Auth.CurrentUser.UserId);

                if (TicketCoroutine == null)
                    TicketCoroutine = StartCoroutine(UpdateTicketCoroutine());
            }
            else
            {
                Debug.Log("로그아웃 상태");

                if (TicketCoroutine != null)
                {
                    StopCoroutine(TicketCoroutine);
                    TicketCoroutine = null;
                }
            }
        }
        /// <summary>
        /// 게임이 실행되고 무한 반복되는 코루틴 메서드 <br/>
        /// 일정 주기마다 RechargeTicketAsync를 실행시킴
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateTicketCoroutine()
        {
            while (true)
            {
                if (CurrentTicket >= MaxRegenTicket || !LastTicketTimestamp.HasValue || LastTicketTimestamp <= 0)
                {
                    TimeUntilNextTicket = TimeSpan.Zero;
                    yield return new WaitForSecondsRealtime(5f); 
                    continue;
                }

                TimeUntilNextTicket = CalculateTimeUntilNextRecharge();

                if (TimeUntilNextTicket <= TimeSpan.Zero)
                {
                    _ = RechargeTicketAsync();
                    yield return new WaitForSecondsRealtime(3f); 
                }
                else
                {
                    yield return new WaitForSecondsRealtime((float)Mathf.Min((float)TimeUntilNextTicket.TotalSeconds, 5f));
                }
            }
        }
        
        /// <summary>
        /// 다음 티켓 충전 시간을 계산하는 메서드
        /// </summary>
        /// <returns></returns>
        private TimeSpan CalculateTimeUntilNextRecharge()
        {
            if (LastTicketTimestamp == null || LastTicketTimestamp <= 0) return TimeSpan.Zero;
            
            long currentLocalTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + serverTimeOffset;
            long timeElapsedSinceLastTime = currentLocalTimeMs - LastTicketTimestamp.Value;

            if (timeElapsedSinceLastTime >= RechargeIntervalMs) return TimeSpan.Zero;
            
            long timeLeftMs = RechargeIntervalMs - timeElapsedSinceLastTime;
            return TimeSpan.FromMilliseconds(timeLeftMs);
        }
    }
}