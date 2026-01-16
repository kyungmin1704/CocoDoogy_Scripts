using CocoDoogy.Tile;
using CocoDoogy.Tutorial;
using CocoDoogy.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CocoDoogy.GameFlow.InGame
{
    public class InGameCameraController : MonoBehaviour
    {
        /// <summary>
        /// 화면 최소각
        /// </summary>
        private const float FOV_MIN = 20;
        /// <summary>
        /// 화면 최대각
        /// </summary>
        private const float FOV_MAX = 80;
        /// <summary>
        /// 화면 기준각
        /// </summary>
        private const float FOV_PIVOT = 60;
        /// <summary>
        /// 기준각 기준 카메라 이동속도
        /// </summary>
        private const float PIVOT_WORLD_DISTANCE = 8;
        
        
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera mainCamera;


        void Awake()
        {
            MapSaveLoader.OnMapLoaded += InitCameraPos;
        }

        void OnDestroy()
        {
            MapSaveLoader.OnMapLoaded -= InitCameraPos;
        }

        void Update()
        {
            if (TouchSystem.IsPointerOverUI) return;
            if (TutorialLocker.CameraLock) return;

            Move();
            Zoom();
        }


        private void InitCameraPos()
        {
            cameraPivot.transform.position = (HexTileMap.MaxPoint + HexTileMap.MinPoint) * 0.5f;
            mainCamera.fieldOfView = FOV_PIVOT;
            ChangeCameraMoveRate();
        }


        #region ◇ CameraMoveRate ◇
        /// <summary>
        /// 카메라 이동비<br/>
        /// FOV를 비례해서 계산
        /// </summary>
        private float cameraMoveRate = 1f;
        
        
        private void ChangeCameraMoveRate()
        {
            cameraMoveRate = mainCamera.fieldOfView / FOV_PIVOT * PIVOT_WORLD_DISTANCE;
        }
        #endregion
        

        #region ◇ Move ◇
        private bool hasMoving = false;
        private Vector2 pivotTouchPos = Vector2.zero;
        private Vector3 pivotCameraPos = Vector3.zero;
        private int lastTouchCount = 0;
        
        
        /// <summary>
        /// 카메라 이동 처리
        /// </summary>
        private void Move()
        {
            //==================
            // Touch Began
            //==================
            if(TouchSystem.TouchCount > 0 && lastTouchCount != TouchSystem.TouchCount)
            {
                hasMoving = true;
                lastTouchCount = TouchSystem.TouchCount;
                pivotTouchPos = TouchSystem.TouchAverage;
                pivotCameraPos = cameraPivot.transform.position;
                return;
            }
            //==================
            // Touch Ended
            //==================
            else if(hasMoving && TouchSystem.TouchCount <= 0)
            {
                hasMoving = false;
                lastTouchCount = TouchSystem.TouchCount;
                return;
            }


            //==================
            // Touch ing
            //==================
            if(!hasMoving) return;
            Vector2 delta = TouchSystem.TouchAverage - pivotTouchPos;
            Vector3 pos = pivotCameraPos - cameraMoveRate * new Vector3(delta.x, 0, delta.y) / Screen.height;

            // 카메라 위치 제약
            pos = Vector3.Min(pos, HexTileMap.MaxPoint);
            cameraPivot.transform.position = Vector3.Max(pos, HexTileMap.MinPoint);
        }
        #endregion
        
        
        #region ◇ Zoom ◇
        private bool hasZooming = false;
        private float pivotDistance = 0f;
        private float pivotFov = 0f;
        
        
        /// <summary>
        /// 카메라 확대/축소 처리
        /// </summary>
        private void Zoom()
        {
            //==================
            // Mouse Zoom
            //==================
            if(TouchSystem.CurrentInputType == TouchSystem.InputType.Mouse)
            {
                float scroll = Mouse.current.scroll.ReadValue().y * 10;

                if (Mathf.Abs(scroll) > 0.1f)
                {
                    mainCamera.fieldOfView -= scroll;
                    mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, FOV_MIN, FOV_MAX);
                    ChangeCameraMoveRate();
                }
                return;
            }


            //==================
            // TouchScreen Zoom
            //==================
            if(!hasZooming && TouchSystem.TouchCount >= 2) // Touch Began
            {
                hasZooming = true;
                pivotDistance = TouchSystem.DistanceAverage;
                pivotFov = mainCamera.fieldOfView;
                return;
            }
            else if(hasZooming && TouchSystem.TouchCount < 2) // Touch Ended
            {
                hasZooming = false;
                return;
            }

            if(!hasZooming) return;
            float rate = pivotDistance / TouchSystem.DistanceAverage;
            float fov = pivotFov * rate;
            
            if (Mathf.Approximately(fov, mainCamera.fieldOfView)) return;
            mainCamera.fieldOfView = Mathf.Clamp(fov, 20, 80);
            ChangeCameraMoveRate();
        }
        #endregion
    }
}