using CocoDoogy.Core;
using FMODUnity;

namespace CocoDoogy.Audio
{
    public enum BgmType
    {
        None = 0,
        LobbyBgm,
        ForestBgm,
        WateryBgm,
        WinterBgm,
        DesertBgm,
        //Bgm 추가하면 여기에도 추가
    }

    [System.Serializable]
    public struct BgmReference
    {
        public BgmType type;
        public EventReference eventReference;
    }

    [System.Serializable]
    public struct ThemeBgm
    {
        public Theme theme;
        public BgmType bgmType;
    }
}