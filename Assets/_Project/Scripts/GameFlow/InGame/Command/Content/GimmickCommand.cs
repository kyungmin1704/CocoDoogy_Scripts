using CocoDoogy.Audio;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 기믹과 관련된 처리를 하는 Command<br/>
    /// GridPos(Vector2Int), Gimmick(GimmickType), MainData(int), SubData(int), PreSubData(int), Dir(HexDirection), PreDIr(HexDirection), DidGimmick(bool)<br/>
    /// 
    /// </summary>
    public class GimmickCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [SerializeField] private Vector2Int gp = Vector2Int.zero;
        [SerializeField] private GimmickType gt = GimmickType.None;
        [SerializeField] private int md = 0;
        [SerializeField] private int sd = 0;
        [SerializeField] private int psd = 0;
        [SerializeField] private HexDirection nd = HexDirection.East;
        [SerializeField] private HexDirection pd = HexDirection.East;
        [SerializeField] private byte dg = 0;


        public Vector2Int GridPos { get => gp; private set => gp = value; }
        public GimmickType Gimmick { get => gt; private set => gt = value; }
        public int MainData { get => md; private set => md = value; }
        public int SubData { get => sd; private set => sd = value; }
        public int PreSubData { get => psd; private set => psd = value; }
        public HexDirection NextDir { get => nd; private set => nd = value; }
        public HexDirection PreDir { get => pd; private set => pd = value; }
        public bool DidGimmick { get => dg == 1; private set => dg = value ? (byte)1 : (byte)0; }
        
        
        public GimmickCommand(object param) : base(CommandType.Gimmick)
        {
            var data = ((Vector2Int, GimmickType, int, int, int, HexDirection, HexDirection, bool))param;
            GridPos = data.Item1;
            Gimmick = data.Item2;
            MainData = data.Item3;
            SubData = data.Item4;
            PreSubData = data.Item5;
            NextDir = data.Item6;
            PreDir = data.Item7;
            DidGimmick = data.Item8;
        }

        
        public override void Execute()
        {
            HexTile tile = HexTile.GetTile(GridPos);
            GimmickData gimmick = HexTileMap.GetGimmick(GridPos);
            if (DidGimmick && gimmick != null)
            {
                gimmick.IsOn = !gimmick.IsOn;
            }
                
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    tile.Rotate((HexRotate)MainData);
                    SfxManager.PlaySfx(SfxType.Gimmick_Mechanical);
                    break;
                case GimmickType.PieceChange:
                    tile.SetPiece((HexDirection)MainData, (PieceType)SubData, NextDir);
                    SfxManager.PlaySfx(SfxType.Gimmick_ObjectSpawn);
                    break;
                case GimmickType.PieceMove:
                    tile.GetPiece((HexDirection)MainData).Move(NextDir);
                    break;
            }
        }

        public override void Undo()
        {
            HexTile tile = HexTile.GetTile(GridPos);
            GimmickData gimmick = HexTileMap.GetGimmick(GridPos);
            if (DidGimmick && gimmick != null)
            {
                gimmick.IsOn = !gimmick.IsOn;
            }
            
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    tile.Rotate((HexRotate)(-MainData));
                    break;
                case GimmickType.PieceChange:
                    tile.SetPiece((HexDirection)MainData, (PieceType)PreSubData, PreDir);
                    break;
                case GimmickType.PieceMove:
                    tile = HexTile.GetTile(GridPos.GetDirectionPos(NextDir));
                    tile.GetPiece((HexDirection)MainData).Move(PreDir);
                    break;
            }
        }
    }
}