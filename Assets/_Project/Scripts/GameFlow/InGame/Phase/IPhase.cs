namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 한 행동의 처리 과정을 작업
    /// </summary>
    public interface IPhase
    {
        /// <summary>
        /// 작업 순서
        /// </summary>
        /// <returns>다음 작업 진행이 가능한지</returns>
        public bool OnPhase();
    }
}