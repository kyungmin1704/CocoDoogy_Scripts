namespace CocoDoogy
{
    /// <summary>
    /// 범용적으로 사용하는 상수를 위한 class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Mathf.Sqrt(3)의 근삿값
        /// </summary>
        public const float SQRT_3 = 1.73205080756877f;
        /// <summary>
        /// 회전 Tween Duration
        /// </summary>
        public const float ROTATE_DURATION = 0.25f;
        /// <summary>
        /// 타일 이동 시간
        /// </summary>
        public const float MOVE_DURATION = 0.5f;
        /// <summary>
        /// 슬라이드 당 이동 시간
        /// </summary>
        public const float SLIDE_PER_DURATION = 0.2f;



        public static class Prefs
        {
            public const string LOBBY_THEME = "LobbyTheme";
        }
    }
}