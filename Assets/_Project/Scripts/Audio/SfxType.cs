using FMODUnity;

namespace CocoDoogy.Audio
{
    public enum SfxType
    {
        //라이더의 초록줄은 언더바 때문이니 무시하세요
        None = 0,
        //상호 작용 계열 - event:/Sfx/Interaction/
        Interaction_LeverOn,
        Interaction_LeverOff,
        Interaction_SwitchOn,
        Interaction_SwitchOff,
        Interaction_PressurePlate,
        Interaction_PushChest,
        Interaction_Sliding,
        Interaction_WaterSplash,
        
        //발자국 소리 - event:/Sfx/Footstep/
        Footstep_Water,
        Footstep_Grass,
        Footstep_Snow,
        Footstep_Sand,
        Footstep_Wood,
        Footstep_Dirt,
        
        //이벤트 개시 계열 - event:/Sfx/Gimmick/
        Gimmick_HouseEnter,
        Gimmick_OasisEnter,
        Gimmick_DockEnter,
        Gimmick_ObjectDestroy,
        Gimmick_ObjectSpawn,
        Gimmick_Farm,
        Gimmick_Mechanical,
        
        //날씨 이벤트 - event:/Sfx/Weather/
        Weather_Clear,
        Weather_Rain,
        Weather_Snow,
        Weather_Wind,
        Weather_Hail,
        Weather_Mirage,
        
        //미니게임 - event:/Sfx/Minigame/
        Minigame_PickTrash,
        Minigame_DropTrash,
        Minigame_PickCloth,
        Minigame_DropCloth,
        Minigame_DigSand,
        Minigame_ShakeUmbrella,
        Minigame_PickLeaf,
        Minigame_MinigameStart,
        Minigame_TouchTrashCan,
        
        //UI계열 - event:/Sfx/UI/
        UI_SuccessMission,
        UI_SuccessStage,
        UI_Success,
        UI_FailStage,
        UI_ButtonDown,
        UI_ButtonUp1,
        UI_ButtonUp2,
        UI_PopUp,
        UI_FailButton1,
        UI_FailButton2,
        UI_ClearStar1,
        UI_ClearStar2,
        UI_ClearStar3,
        UI_Reset,
        
        //감정표현 계열 - event:/Sfx/Emote/
        Emote_Positive,
        Emote_Neutral,
        Emote_Negative,
        
        //아이템 계열 - event:/Sfx/Item/
        Item_Eating,
        Item_TentDrop,
        Item_Undo,
        Item_Tent,
        Item_Recovery,
        Item_RestorePoint,
        Item_DogSleeping,
        
        //Loop계열 - event:/Sfx/Loop/
        Loop_Detecting,
        Loop_ShakeUmbrella
    }
    
    [System.Serializable]
    public struct SfxReference
    {
        public SfxType type;
        public EventReference eventReference;
    }
}
