using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CocoDoogy.MapEditor.Controller
{
    public class MapEditorInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;



        public static event Action<Vector2> OnMove = null;
        public static event Action<bool> OnLeftControl = null;
        
        public static event Action<bool> OnLeftClick = null;
        public static event Action<bool> OnRightClick = null;
        public static event Action<float> OnZoom = null;
        public static event Action OnSave = null;
        public static event Action OnLoad = null;
        public static event Action OnClear = null;
        public static event Action OnToggleUI = null;
        public static event Action OnToggleMode = null;
        public static event Action OnLeftRotate = null;
        public static event Action OnRightRotate = null;
        
        
        void Awake()
        {
            if (playerInput == null) playerInput = FindAnyObjectByType<PlayerInput>();

            InputActionMap actionMap = playerInput.actions.FindActionMap("MapEditor");
            actionMap.AddAction("Move", OnMoveChanged);
            actionMap.AddAction("LeftControl", OnLeftControlClick, null, OnLeftControlClick);
            
            actionMap.AddAction("LeftClick", OnLeftMouseClick, null, OnLeftMouseClick);
            actionMap.AddAction("RightClick", OnRightMouseClick, null, OnRightMouseClick);
            actionMap.AddAction("Zoom", OnZoomChanged);
            
            actionMap.AddAction("Save", OnSaveButtonClick, null, null);
            actionMap.AddAction("Load", OnLoadButtonClick, null, null);
            actionMap.AddAction("Clear", OnClearButtonClick, null, null);
            
            actionMap.AddAction("ToggleUI", OnToggleUIChange, null, null);
            actionMap.AddAction("ToggleMode", OnToggleModeChange, null, null);
            actionMap.AddAction("LeftRotate", OnLeftRotateClick, null, null);
            actionMap.AddAction("RightRotate", OnRightRotateClick, null, null);
        }
        void OnDestroy()
        {
            try
            {
                InputActionMap actionMap = playerInput.actions.FindActionMap("MapEditor");
                actionMap.RemoveAction("Move", OnMoveChanged);
                actionMap.RemoveAction("LeftControl", OnLeftControlClick, null, OnLeftControlClick);
            
                actionMap.RemoveAction("LeftClick", OnLeftMouseClick, null, OnLeftMouseClick);
                actionMap.RemoveAction("RightClick", OnRightMouseClick, null, OnRightMouseClick);
                actionMap.RemoveAction("Zoom", OnZoomChanged);
            
                actionMap.RemoveAction("Save", OnSaveButtonClick, null, null);
                actionMap.RemoveAction("Load", OnLoadButtonClick, null, null);
                actionMap.RemoveAction("Clear", OnClearButtonClick, null, null);
            
                actionMap.RemoveAction("ToggleUI", OnToggleUIChange, null, null);
                actionMap.RemoveAction("ToggleMode", OnToggleModeChange, null, null);
                actionMap.RemoveAction("LeftRotate", OnLeftRotateClick, null, null);
                actionMap.RemoveAction("RightRotate", OnRightRotateClick, null, null);
            }
            catch
            {
                // ignored
            }
        }


        private void OnMoveChanged(InputAction.CallbackContext context)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnLeftControlClick(InputAction.CallbackContext context)
        {
            OnLeftControl?.Invoke(context.started);
        }

        private void OnLeftMouseClick(InputAction.CallbackContext context)
        {
            OnLeftClick?.Invoke(context.started);
        }
        private void OnRightMouseClick(InputAction.CallbackContext context)
        {
            OnRightClick?.Invoke(context.started);
        }

        private void OnZoomChanged(InputAction.CallbackContext context)
        {
            OnZoom?.Invoke(context.ReadValue<Vector2>().y);
        }
        
        private void OnSaveButtonClick(InputAction.CallbackContext context)
        {
            OnSave?.Invoke();
        }
        private void OnLoadButtonClick(InputAction.CallbackContext context)
        {
            OnLoad?.Invoke();
        }
        private void OnClearButtonClick(InputAction.CallbackContext context)
        {
            OnClear?.Invoke();
        }
        
        private void OnToggleUIChange(InputAction.CallbackContext context)
        {
            OnToggleUI?.Invoke();
        }
        private void OnToggleModeChange(InputAction.CallbackContext context)
        {
            OnToggleMode?.Invoke();
        }
        private void OnLeftRotateClick(InputAction.CallbackContext context)
        {
            OnLeftRotate?.Invoke();
        }
        private void OnRightRotateClick(InputAction.CallbackContext context)
        {
            OnRightRotate?.Invoke();
        }
    }
}
