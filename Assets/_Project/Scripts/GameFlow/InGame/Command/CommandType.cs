namespace CocoDoogy.GameFlow.InGame.Command
{
    public enum CommandType
    {
        /// <summary>
        /// 없다는 개념
        /// </summary>
        None = 0,

        #region ◇ 유저 조작 ◇
        /// <summary>
        /// 이동 처리
        /// </summary>
        Move = 1,
        /// <summary>
        /// 트리거 처리
        /// </summary>
        Trigger = 2,
        #endregion

        #region ◇ 유저비 조작 이동 ◇
        /// <summary>
        /// 미끄러짐 처리
        /// </summary>
        Slide = 21,
        /// <summary>
        /// 순간이동처리
        /// </summary>
        Teleport = 22,
        /// <summary>
        /// 부두술을 이용한 항해
        /// </summary>
        Sail = 23,
        #endregion
        
        #region ◇ 시스템 조작 ◇
        /// <summary>
        /// 유닛 배치
        /// </summary>
        Deploy = 101,
        /// <summary>
        /// 행동력 초기화
        /// </summary>
        Refill = 102,
        /// <summary>
        /// 모래 카운터
        /// </summary>
        SandCount = 103,
        
        /// <summary>
        /// 날씨 처리
        /// </summary>
        Weather = 111,
        
        /// <summary>
        /// 기믹 동작
        /// </summary>
        Gimmick = 121,
        
        /// <summary>
        /// ActionPoints 회복
        /// </summary>
        Increase = 122,

        /// <summary>
        /// Refill 시, 부두의 배 리턴
        /// </summary>
        DeckReset = 123,
        
        #endregion

        #region ◇ 아이템 ◇
        /// <summary>
        /// 아이템 사용 1<br/>
        /// 현재 행동력 1 감소, 최대 행동력 1 증가
        /// </summary>
        MaxUp = 200,
        
        /// <summary>
        /// 아이템 사용 2<br/>
        /// 현재 행동력 1 증가
        /// </summary>
        Recover = 201,
        
        /// <summary>
        /// 아이템 사용 3<br/>
        /// 행동을 한 이력이 있을 때 아이템을 사용하면 1턴 전으로 돌아감
        /// </summary>
        Undo = 202
        #endregion
        
    }
}