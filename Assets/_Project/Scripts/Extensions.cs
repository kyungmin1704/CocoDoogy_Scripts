using CocoDoogy.Tile;
using CocoDoogy.LifeCycle;
using CocoDoogy.Tile.Piece;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CocoDoogy.Core;

namespace CocoDoogy
{
    /// <summary>
    /// 확장 메소드를 위한 class
    /// </summary>
    public static class Extensions
    {
        #region ◇ LifeCycle ◇
        public static Action<T> GetEvents<T>(this IEnumerable<IInit<T>> inits)
        {
            Action<T> result = null;
            foreach (var v in inits)
            {
                result += v.OnInit;
            }
            return result;
        }
        public static Action<T> GetEvents<T>(this IEnumerable<ISpawn<T>> spawns)
        {
            Action<T> result = null;
            foreach (var v in spawns)
            {
                result += v.OnSpawn;
            }
            return result;
        }
        public static Action<T> GetEvents<T>(this IEnumerable<IRelease<T>> releases)
        {
            Action<T> result = null;
            foreach (var v in releases)
            {
                result += v.OnRelease;
            }
            return result;
        }
        public static Action<string> GetInserts(this IEnumerable<ISpecialPiece> inserts)
        {
            Action<string> result = null;
            foreach (var v in inserts)
            {
                result += v.OnDataInsert;
            }
            return result;
        }
        public static Action GetExecutes(this IEnumerable<ISpecialPiece> inserts)
        {
            Action result = null;
            foreach (var v in inserts)
            {
                result += v.OnExecute;
            }
            return result;
        }
        #endregion

        #region ◇ ActionMap ◇
        public static void AddAction(this InputActionMap actionMap, string actionName, Action<InputAction.CallbackContext> callback)
        {
            InputAction inputAction = actionMap.FindAction(actionName);
            inputAction.started += callback;
            inputAction.performed += callback;
            inputAction.canceled += callback;
        }
        public static void AddAction(this InputActionMap actionMap, string actionName,
            Action<InputAction.CallbackContext> started, Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> canceled)
        {
            InputAction inputAction = actionMap.FindAction(actionName);
            if (started != null)
            {
                inputAction.started += started;
            }
            if (performed != null)
            {
                inputAction.performed += performed;
            }
            if (canceled != null)
            {
                inputAction.canceled += canceled;
            }
        }

        public static void RemoveAction(this InputActionMap actionMap, string actionName, Action<InputAction.CallbackContext> callback)
        {
            InputAction inputAction = actionMap.FindAction(actionName);
            inputAction.started -= callback;
            inputAction.performed -= callback;
            inputAction.canceled -= callback;
        }
        public static void RemoveAction(this InputActionMap actionMap, string actionName,
            Action<InputAction.CallbackContext> started, Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> canceled)
        {
            InputAction inputAction = actionMap.FindAction(actionName);
            if (started != null)
            {
                inputAction.started -= started;
            }
            if (performed != null)
            {
                inputAction.performed -= performed;
            }
            if (canceled != null)
            {
                inputAction.canceled -= canceled;
            }
        }
        #endregion

        #region ◇ HexTile ◇
        #region ◇◇ From Vector ◇◇
        /// <summary>
        /// GridPos에서 Direction의 GridPos값을 반환
        /// </summary>
        /// <param name="gridPos">좌표 값</param>
        /// <param name="direction">방향</param>
        /// <returns></returns>
        public static Vector2Int GetDirectionPos(this Vector2Int gridPos, HexDirection direction)
        {
            Vector2Int result = gridPos;
            bool isEven = gridPos.y % 2 == 0;
            
            // 짝수는 NorthEast, SouthEast의 x 변동이 없고,
            // 홀수는 NorthWest, SouthWest의 x 변동이 없음.
            result.x += direction switch
            {
                HexDirection.NorthEast => isEven?0:1,
                HexDirection.East => 1,
                HexDirection.SouthEast => isEven?0:1,
                HexDirection.SouthWest => isEven?-1:0,
                HexDirection.West => -1,
                HexDirection.NorthWest => isEven?-1:0,
                _ => 0,
            };
            result.y += direction switch
            {
                HexDirection.NorthEast => 1,
                HexDirection.NorthWest => 1,
                HexDirection.SouthEast => -1,
                HexDirection.SouthWest => -1,
                _ => 0,
            };
            return result;
        }

        /// <summary>
        /// 목표가 인접했다면, 현재 위치에서 어느 방향에 존재하는지 방향 반환
        /// </summary>
        /// <param name="gridPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static HexDirection? GetRelativeDirection(this Vector2Int gridPos, Vector2Int targetPos)
        {
            for (int i = 0; i < 6; i++)
            {
                HexDirection direction = (HexDirection)i;
                if (targetPos == gridPos.GetDirectionPos(direction))
                {
                    return direction;
                }
            }
            return null;
        }

        /// <summary>
        /// 6방향의 GridPos값을 반환
        /// </summary>
        /// <param name="gridPos">중심점</param>
        /// <returns></returns>
        public static Vector2Int[] GetAdjacentGridPoses(this Vector2Int gridPos)
        {
            Vector2Int[] result = new Vector2Int[6];
            for (int i = 0; i < 6; i++)
            {
                gridPos.GetDirectionPos((HexDirection)i);
            }

            return result;
        }

        /// <summary>
        /// Grid 좌표 값을 World Position으로 변경
        /// </summary>
        /// <param name="gridPos">좌표 값</param>
        /// <returns></returns>
        public static Vector3 ToWorldPos(this Vector2Int gridPos) => new Vector3(gridPos.x + (gridPos.y % 2 == 0 ? 0f : 0.5f), 0, gridPos.y * 0.5f * Constants.SQRT_3);

        /// <summary>
        /// Grid 좌표 값을 World Position으로 변경
        /// </summary>
        /// <param name="worldPos">실수 좌표 값</param>
        /// <returns></returns>
        public static Vector2Int ToGridPos(this Vector3 worldPos)
        {
            int y = Mathf.RoundToInt(worldPos.z / Constants.SQRT_3 * 2);
            int x = Mathf.RoundToInt(worldPos.x - (y % 2 == 0 ? 0 : 0.5f));
            
            return new Vector2Int(x, y);
        }
        #endregion

        #region ◇◇ From Hex~ ◇◇
        /// <summary>
        /// Hex 회전 값을 World Rotation으로 변경
        /// </summary>
        /// <param name="rotate">회전 값</param>
        /// <returns></returns>
        public static float ToDegree(this HexRotate rotate) => (int)rotate * -60f;
        /// <summary>
        /// Hex 방향 값을 World Rotation으로 변경
        /// </summary>
        /// <param name="direction">방향 값</param>
        /// <returns></returns>
        public static float ToDegree(this HexDirection direction) => (int)direction * -60f;

        /// <summary>
        /// HexDirection이 마주보는 반대 방향을 반환
        /// </summary>
        /// <returns>반대 방향</returns>
        public static HexDirection GetMirror(this HexDirection direction) => (HexDirection)(((int)direction + 3) % 6);

        /// <summary>
        /// HexRotate의 반대 회전으로 반환
        /// </summary>
        /// <returns>반대 방향</returns>
        public static HexRotate GetMirror(this HexRotate rotate) => (HexRotate)(-(int)rotate);
        
        /// <summary>
        /// 기물의 상대 위치<br/>
        /// GetPiecePosition Method전용
        /// </summary>
        private static readonly Vector3[] PiecePositions = 
        {
            new(0.5f, 0f, 0f),      // 동
            new(0.25f, 0f, 0.75f / Constants.SQRT_3),  // 북동
            new(-0.25f, 0f, 0.75f / Constants.SQRT_3), // 북서
            new(-0.5f, 0f, 0f),     // 서
            new(-0.25f, 0f, -0.75f / Constants.SQRT_3),// 남서
            new(0.25f, 0f, -0.75f / Constants.SQRT_3), // 남동
            new(0f, 0f, 0f),    // 중앙
        };
        /// <summary>
        /// 기물의 상대 위치를 계산
        /// </summary>
        /// <param name="direction">기물의 방향</param>
        /// <returns></returns>
        public static Vector3 GetPos(this HexDirection direction) => PiecePositions[(int)direction];

        /// <summary>
        /// 기존 방향에 회전값을 더한 뒤에 방향을 반환
        /// </summary>
        /// <param name="direction">기물의 방향</param>
        /// <param name="rotate">회전</param>
        /// <returns></returns>
        public static HexDirection AddRotate(this HexDirection direction, HexRotate rotate)
        {
           int result = (int)direction + (int)rotate;
           result = (result < 0 ? result + 6 : result) % 6;

           return (HexDirection)result;
        }
        #endregion
        #endregion


        /// <summary>
        /// Plane.Raycast를 float가 아닌, Vector3가 나오게 하는 확장 메소드
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="ray"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Raycast(this Plane plane, Ray ray, out Vector3 point)
        {
            bool result = plane.Raycast(ray, out float distance);
            point = ray.GetPoint(distance);
            
            return result;
        }

        /// <summary>
        /// Theme를 Indexing이 유리한 형태로 변경
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static int ToIndex(this Theme theme) => theme switch
        {
            Theme.Forest => 0,
            Theme.Water => 1,
            Theme.Snow => 2,
            Theme.Sand => 3,
            _ => -1
        };
        public static string ToName(this Theme theme) => theme switch
        {
            Theme.Forest => "숲",
            Theme.Water => "물",
            Theme.Snow => "눈",
            Theme.Sand => "사막",
            _ => "무지개랜드"
        };

        /// <summary>
        /// Enable을 Method형태로 제공
        /// </summary>
        /// <param name="component"></param>
        /// <param name="enable"></param>
        public static void SetEnable(this MonoBehaviour component, bool enable)
        {
            component.enabled = enable;
        }

        /// <summary>
        /// 정수가 min과 max 사이에 존재하는 수인지 확인
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(this int value, int min, int max)
        {
            return min <= value && value <= max;
        }

        /// <summary>
        /// int값을 2자리의 16진수로 변환하여 string 값으로 반환하는 메서드 <br/>
        /// ex) 15 -> 0F, 26 -> 1A ... 로 변환하여 반환함.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Hex2(this int value)
        {
            return value.ToString("X2"); 
        }
        /// <summary>
        /// 2자리의 16진수로 변환된 string 값을 정수로 변환하는 메서드 <br/>
        /// ex) 0F > 15, 1A -> 26 ... 로 변환하여 반환함.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Hex2Int(this string value)
        {
            return Convert.ToInt32(value, 16);
        }
    }
}