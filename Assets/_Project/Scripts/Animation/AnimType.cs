namespace CocoDoogy.Animation
{
    /// <summary>
    /// Player Animation State Type
    /// </summary>
    public enum AnimType
    {
        /// <summary>
        /// 기본
        /// </summary>
        Idle = 0,
        /// <summary>
        /// 이동
        /// </summary>
        Moving = 1,
        /// <summary>
        /// 미끄러짐
        /// </summary>
        Slide = 2,
        /// <summary>
        /// 상호작용
        /// </summary>
        Interaction = 3,
        /// <summary>
        /// 고개 갸웃
        /// </summary>
        Curious = 4,
        /// <summary>
        /// 착석
        /// </summary>
        Sit = 5,
    }
}
