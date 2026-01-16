namespace CocoDoogy.MapEditor.Controller
{
    public enum MapEditMode
    {
        /// <summary>
        /// 선택 안 함
        /// </summary>
        None = 0,

        /// <summary>
        /// 타일 선택 모드
        /// </summary>
        SelectMode = 1,
        /// <summary>
        /// 타일 배치 모드
        /// </summary>
        InsertMode = 2,
        /// <summary>
        /// 타일 삭제 모드
        /// </summary>
        DeleteMode = 3,

        /// <summary>
        /// 시작점 지정 모드
        /// </summary>
        StartPosMode = 6,
        /// <summary>
        /// 끝 점 지정 모드
        /// </summary>
        EndPosMode = 7,

        /// <summary>
        /// 기믹의 트리거 타일 지정 모드
        /// </summary>
        GimmickTriggerMode = 11,
        /// <summary>
        /// 특수 기물의 타겟 지정 모드
        /// </summary>
        PieceTargetMode = 12,
    }
}