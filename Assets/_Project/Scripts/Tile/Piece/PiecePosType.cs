using System;

namespace CocoDoogy.Tile.Piece
{
    /// <summary>
    /// 기물의 설치 가능 위치
    /// </summary>
    [Flags]
    public enum PiecePosType
    {
        None    = 0,
        Side    = 1 << 0,
        Center  = 1 << 1,
    }
}