namespace CocoDoogy.Data
{
    public enum ItemEffect
    {
        None,
        /// <summary>
        /// 행동력 1을 소모하고 최대 행동력 1을 증가
        /// </summary>
        ConsumeAndRecoverMaxAP,
        /// <summary>
        /// 행동력을 1증가
        /// </summary>
        RecoverAP,
        /// <summary>
        /// 1턴 전으로 이동
        /// </summary>
        UndoTurn,
    }
}