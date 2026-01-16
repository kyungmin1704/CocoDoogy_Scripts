namespace CocoDoogy.Tile.Piece
{
    public enum PieceType
    {
        None = 0,

        // 트리거 그룹
        /// <summary>
        /// On/Off 레버
        /// </summary>
        Lever = 001,
        /// <summary>
        /// 일정시간동안 동작하는 Trigger형 버튼
        /// </summary>
        Button = 002,
        /// <summary>
        /// 위에 물체가 있을 때만 동작하는 발판형 버튼
        /// </summary>
        GravityButton = 003,
        /// <summary>
        /// 부두<br/>
        /// 부두술로 배를 움직일 수 있음
        /// </summary>
        Deck = 004,
        /// <summary>
        /// 상자
        /// </summary>
        Crate = 005,
        /// <summary>
        /// 물 위에 뜬 상자
        /// </summary>
        FloatedCrate = 006,
        /// <summary>
        /// 발판형 버튼 위에 상자
        /// </summary>
        GravityCrate = 007,

        // 오브젝트
        /// <summary>
        /// 다리
        /// </summary>
        Bridge = 101,
        /// <summary>
        /// 큰 돌
        /// </summary>
        BigRock = 102,
        /// <summary>
        /// 작은 돌
        /// </summary>
        SmallRock = 103,
        /// <summary>
        /// 나무
        /// </summary>
        Tree = 104,
        /// <summary>
        /// 밭
        /// </summary>
        Field = 105,
        /// <summary>
        /// 오아시스
        /// </summary>
        Oasis = 106,
        /// <summary>
        /// 집
        /// </summary>
        House = 107,
        /// <summary>
        /// 숲
        /// </summary>
        Forest = 108,
        /// <summary>
        /// 선인장
        /// </summary>
        Cactus = 109,
        
        /// <summary>
        /// 강풍
        /// </summary>
        Tornado = 201,
    }
}