namespace CocoDoogy.Core
{
    [System.Flags]
    public enum Theme
    {
        //게임을 만들때 매개변수로 받으면서 배경의 이미지를 바꾸도록
        None = 0,
        Forest = 1 << 0,
        Water = 1 << 1,
        Snow = 1 << 2,
        Sand = 1 << 3,
    }
}