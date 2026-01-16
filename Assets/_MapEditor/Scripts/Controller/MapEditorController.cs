using CocoDoogy.MapEditor.UI;
using CocoDoogy.MapEditor.UI.GimmickConnector;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CocoDoogy.MapEditor.Controller
{
    public class MapEditorController : MonoBehaviour
    {
        public static TileType SelectedTileType { get; set; } = TileType.None;
        public static MapEditMode EditMode
        {
            get => editMode;
            set => OnEditModeChanged?.Invoke(editMode = value);
        }
        
        
        private static readonly Plane Plane = new(Vector3.up, Vector3.zero);
        private static MapEditMode editMode = MapEditMode.None;
        
        public static event Action<MapEditMode> OnEditModeChanged = null;


        [Header("Camera View (2D Edit, 3D View)")]
        [SerializeField] private Camera topDownCamera;
        [SerializeField] private Camera inGameCamera;

        [Header("Panels")]
        [SerializeField] private PieceDeployPanel piecePanel;
        [SerializeField] private GimmickConnectPanel gimmickPanel;


        /// <summary>
        /// 현재 프레임에 마우스 커서가 UI 위에 있는지
        /// </summary>
        private bool HasOverUI { get; set; } = true;
        /// <summary>
        /// Edit 가능한 상태인지
        /// </summary>
        private bool CanEdit => isEditCameraMode && !HasOverUI;
        
        
        private HexTile selectedTile = null;
        private readonly List<Vector2Int> canMoveTiles = new();
        
        private bool isEditCameraMode = true;
        private bool isEditing = false;

        private Vector2Int? mouseOverGridPos = null;
        
        
        #region ◇ Unity Events ◇
        void Awake()
        {
            OnEditModeChanged += OnNotEditModeSelect;

            // 조작 연결
            MapEditorInputHandler.OnLeftClick += OnLeftClick;
            MapEditorInputHandler.OnRightClick += OnRightClick;
            MapEditorInputHandler.OnZoom += OnZoom;
            
            MapEditorInputHandler.OnClear += HexTileMap.Clear;
            MapEditorInputHandler.OnToggleMode += OnCameraModeChange;

            // 맵 로드 이벤트 연결
            MapSaveLoader.OnMapLoaded += ClearSelectedTile;
        }

        void OnDestroy()
        {
            OnEditModeChanged -= OnNotEditModeSelect;

            // 조작 해제
            MapEditorInputHandler.OnLeftClick -= OnLeftClick;
            MapEditorInputHandler.OnRightClick -= OnRightClick;
            MapEditorInputHandler.OnZoom -= OnZoom;
            
            MapEditorInputHandler.OnClear -= HexTileMap.Clear;
            MapEditorInputHandler.OnToggleMode -= OnCameraModeChange;
            
            // 맵 로드 이벤트 해제
            MapSaveLoader.OnMapLoaded -= ClearSelectedTile;
        }

        private void Update()
        {
            // 마우스 커서가 UI 위에 올라간 상태인지 판정
            HasOverUI = EventSystem.current is null || EventSystem.current.IsPointerOverGameObject();

            EditTile();
        }
        #endregion


        #region ◇ Tile Edits ◇
        /// <summary>
        /// 타일 작업과 관련된 처리
        /// </summary>
        private void EditTile()
        {
            // 타일 수정 가능 상태인지 체크
            if (!isEditing)
            {
                mouseOverGridPos = null;
                return;
            }
            if (!CanEdit) return;


            if (!Plane.Raycast(GetRay(), out Vector3 point)) return; // 아마 동작할 일 없는 예외처리
            Vector2Int gridPos = point.ToGridPos(); // 커서로 선택된 좌표값

            // 이미 동작을 처리했던 좌표값이면 생략
            if (mouseOverGridPos == gridPos) return;
            mouseOverGridPos = gridPos;

            switch (EditMode)
            {
                case MapEditMode.SelectMode:
                    SelectTile(gridPos);
                    break;
                case MapEditMode.InsertMode:
                    CreateTile(gridPos);
                    break;
                case MapEditMode.DeleteMode:
                    DeleteTile(gridPos);
                    break;
                case MapEditMode.StartPosMode:
                    StartPosTile(gridPos);
                    break;
                case MapEditMode.EndPosMode:
                    EndPosTile(gridPos);
                    break;
                case MapEditMode.GimmickTriggerMode:
                    GimmickTriggerTile(gridPos);
                    break;
                case MapEditMode.PieceTargetMode:
                    TargetTile(gridPos);
                    break;
            }
        }
        /// <summary>
        /// 타일 선택
        /// </summary>
        /// <param name="gridPos"></param>
        private void SelectTile(Vector2Int gridPos)
        {
            ClearSelectedTile();
            selectedTile = HexTile.GetTile(gridPos);
            
            if (!selectedTile) return;
            
            piecePanel.Open(selectedTile);
            selectedTile.DrawOutline(Color.green);

            canMoveTiles.AddRange(selectedTile.CanMovePoses());
            foreach(var gPos in canMoveTiles)
            {
                HexTile.GetTile(gPos).DrawOutline(Color.yellow);
            }
        }
        /// <summary>
        /// 타일 생성
        /// </summary>
        /// <param name="gridPos"></param>
        private void CreateTile(Vector2Int gridPos)
        {
            HexTile spawnedTile = HexTileMap.AddTile(SelectedTileType, gridPos);
        }
        /// <summary>
        /// 타일 제거
        /// </summary>
        /// <param name="gridPos"></param>
        private void DeleteTile(Vector2Int gridPos)
        {
            HexTileMap.RemoveTile(gridPos);
        }
        /// <summary>
        /// 시작점으로 지정
        /// </summary>
        /// <param name="gridPos"></param>
        private void StartPosTile(Vector2Int gridPos)
        {
            if (!HexTile.GetTile(gridPos)) return;

            HexTile.GetTile(HexTileMap.StartPos)?.OffOutline();
            HexTile.GetTile(HexTileMap.StartPos = gridPos).DrawOutline(Color.red);
        }
        /// <summary>
        /// 도착점으로 지정
        /// </summary>
        /// <param name="gridPos"></param>
        private void EndPosTile(Vector2Int gridPos)
        {
            if (!HexTile.GetTile(gridPos)) return;

            HexTile.GetTile(HexTileMap.EndPos)?.OffOutline();
            HexTile.GetTile(HexTileMap.EndPos = gridPos).DrawOutline(Color.purple);
        }
        /// <summary>
        /// 트리거로 지정
        /// </summary>
        /// <param name="gridPos"></param>
        private void GimmickTriggerTile(Vector2Int gridPos)
        {
            HexTile selectedTile = HexTile.GetTile(gridPos);
            if (!selectedTile) return;

            Piece centerPiece = selectedTile.GetPiece(HexDirection.Center);
            if (!centerPiece || !centerPiece.IsTrigger) return;

            ClearSelectedTile();
            gimmickPanel.SelectedGimmick.Triggers.Add(new() { GridPos = gridPos });
            gimmickPanel.OpenFromTile(gimmickPanel.SelectedGimmick.Target.GridPos);
            EditMode = MapEditMode.SelectMode;
        }
        /// <summary>
        /// 목표 타일로 지정
        /// </summary>
        /// <param name="gridPos"></param>
        private void TargetTile(Vector2Int gridPos)
        {
            HexTile selectedTile = HexTile.GetTile(gridPos);
            if (!selectedTile) return;

            foreach (Piece piece in PieceDeployPanel.SelectedTile.Pieces)
            {
                if (!piece || !piece.BaseData.hasTarget) continue;
                
                if (piece.Target != null)
                {
                    HexTile preTile = HexTile.GetTile((Vector2Int)piece.Target);
                    if (preTile)
                    {
                        preTile.OffOutline();
                    }
                }
                piece.Target = gridPos;
            }
            
            ClearOutlines();
            selectedTile.DrawOutline(Color.black);
        }
        #endregion

        
        #region ◇ Input Events ◇
        /// <summary>
        /// Editing을 활성화/비활성화
        /// </summary>
        /// <param name="clicked">활성화 여부</param>
        private void OnLeftClick(bool clicked)
        {
            if (EditMode == MapEditMode.None) return;
            
            isEditing = clicked;
        }

        /// <summary>
        /// 취소를 위한 클릭
        /// </summary>
        /// <param name="clicked"></param>
        private void OnRightClick(bool clicked)
        {
            if (clicked)
            {
                if (EditMode == MapEditMode.None)
                {
                    EditMode = MapEditMode.DeleteMode;
                    isEditing = true;
                }
                return;
            }
            isEditing = false;

            if(selectedTile)
            {
                ClearSelectedTile();
                return;
            }

            if (EditMode == MapEditMode.None) return;

            EditMode = MapEditMode.None;
        }
        /// <summary>
        /// 줌 In/Out
        /// </summary>
        /// <param name="delta"></param>
        private void OnZoom(float delta)
        {
            if (HasOverUI) return;

            if (isEditCameraMode)
            {
                topDownCamera.orthographicSize = Mathf.Clamp(topDownCamera.orthographicSize - delta, 5f, 15f);
            }
            else
            {
                inGameCamera.fieldOfView = Mathf.Clamp(inGameCamera.fieldOfView - delta, 30f, 60f);
            }
        }
        /// <summary>
        /// 카메라의 뷰를 변경하는 메서드 (Edit <-> View) 
        /// </summary>
        private void OnCameraModeChange()
        {
            isEditCameraMode = !isEditCameraMode;
            topDownCamera.gameObject.SetActive(isEditCameraMode);
            inGameCamera.gameObject.SetActive(!isEditCameraMode);
        }
        #endregion

        /// <summary>
        /// EditMode가 SelectMode가 아니라면, 타일 Outline 초기화
        /// </summary>
        /// <param name="editMode"></param>
        private void OnNotEditModeSelect(MapEditMode editMode)
        {
            if (editMode == MapEditMode.SelectMode) return;

            ClearSelectedTile();
        }


        /// <summary>
        /// EditMode Camera에서 Ray 발사
        /// </summary>
        /// <returns></returns>
        private Ray GetRay() => topDownCamera.ScreenPointToRay(Mouse.current.position.value);
        /// <summary>
        /// 선택된 타일 초기화 및 타일들 Outline 초기화 작업
        /// </summary>
        private void ClearSelectedTile()
        {
            if (selectedTile)
            {
                selectedTile.OffOutline();
                selectedTile = null;
            }
            
            ClearOutlines();

            piecePanel.Close();
        }
        private void ClearOutlines()
        {
            foreach (var gPos in canMoveTiles)
            {
                HexTile.GetTile(gPos)?.OffOutline();
            }
            canMoveTiles.Clear();

            foreach (var gPos in HexTileMap.Gimmicks.Keys)
            {
                HexTile.GetTile(gPos).DrawOutline(Color.blue);
            }

            HexTile.GetTile(HexTileMap.StartPos)?.DrawOutline(Color.red);
            HexTile.GetTile(HexTileMap.EndPos)?.DrawOutline(Color.purple);

            foreach(var tile in HexTile.Tiles.Values)
            {
                foreach (Piece piece in tile.Pieces)
                {
                    if (!piece || !piece.BaseData.hasTarget) continue;
                    if (piece.Target == null) continue;
                    
                    HexTile targetTile = HexTile.GetTile(piece.Target.Value);
                    if (!targetTile) continue;
                    targetTile.DrawOutline(Color.black);
                    break;
                }
            }
        }
    }
}