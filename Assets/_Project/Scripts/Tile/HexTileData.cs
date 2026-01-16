using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame;
using UnityEngine;

namespace CocoDoogy.Tile
{
    [CreateAssetMenu(menuName = "Data/Tile Data", fileName = "New Tile Data")]
    public class HexTileData : ScriptableObject
    {
        [Header("Common Data")]
        [Tooltip("Tile의 고유 Type")] public TileType type = TileType.None;
        [Tooltip("Tile 아이콘")] public Sprite tileIcon = null;
        [Tooltip("Tile 명칭")] public string tileName = string.Empty;
        [Tooltip("Tile 설명")][TextArea(3, 10)] public string description = string.Empty;
        [Tooltip("Tile의 시각적 표시용 GameObject")] public GameObject modelPrefab = null;

        [Header("Move Setting")]
        [Tooltip("Tile 기본 이동 가능 여부")] public bool canMove = true;
        [Tooltip("Tile 기본 이동 가능 여부")] public MoveType moveType = MoveType.None;
        [Tooltip("이동 비용")] public int moveCost = 1;

        [Header("Step Effects")]
        [Tooltip("발판에 이동했을 때 나올 VFX")] public VfxType stepVfx = VfxType.None;
        [Tooltip("발판에 이동했을 때 나올 SFX")] public SfxType stepSfx = SfxType.None;

        [Header("Editor")]
        [Tooltip("비고")][TextArea(3, 10)][SerializeField] private string editorDescription = string.Empty;


        public int RealMoveCost => moveCost * (PlayerHandler.SandCount >= 2 ? 2 : 1);
    }
}