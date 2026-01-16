using Newtonsoft.Json;

namespace CocoDoogy.Network.Ticket
{
    public class TicketResponse
    {
        /// <summary>
        /// 예외 상황 없이 조건에 성공했는지 여부
        /// </summary>
        [JsonProperty("success")] public bool Success; 
        /// <summary>
        /// Success가 false 라면 그 이유
        /// </summary>
        [JsonProperty("reason")] public string Reason; 
        /// <summary>
        /// 현재 uid가 가지고 있는 티켓 수
        /// </summary>
        [JsonProperty("gameTicket")] public int CurrentTicket;  
        /// <summary>
        /// 현재 uid가 가지고 있는 추가 티켓 수
        /// </summary>
        [JsonProperty("bonusTicket")] public int BonusTicket;
        /// <summary>
        /// 클라이언트와 서버의 시간 차를 알기 위한 서버 시간
        /// </summary>
        [JsonProperty("serverTime")] public long? ServerTime; 
        /// <summary>
        /// DB에 저장된 마지막으로 티켓이 추가 된 시간
        /// </summary>
        [JsonProperty("lastTicketTime")] public long? LastTicketTime; 
        /// <summary>
        /// 티켓 추가 수
        /// </summary>
        [JsonProperty("added")] public int Added;
    }
}
