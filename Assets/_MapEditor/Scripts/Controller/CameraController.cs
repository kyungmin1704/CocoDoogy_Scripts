using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CocoDoogy.MapEditor.Controller
{
    public class CameraController : MonoBehaviour
    {
        [Header("Move Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private bool lockY = true;
        [SerializeField] private float fixedY = 20f;
    
        
        private bool leftCtrlPressed = false;
        
        private PlayerInput playerInput = null;
        private Vector2 moveDir = Vector2.zero;

        
        void OnEnable()
        {
            moveDir = Vector2.zero;
            MapEditorInputHandler.OnMove += OnMove;
            MapEditorInputHandler.OnLeftControl += OnLeftControl;
        }
        void OnDisable()
        {
            MapEditorInputHandler.OnMove -= OnMove;
            MapEditorInputHandler.OnLeftControl -= OnLeftControl;
        }

        void Update()
        {
            if (leftCtrlPressed) return;
            
            if (lockY)
            {
                transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
            }
            else
            {
                Vector3 move = moveSpeed * Time.deltaTime * new Vector3(moveDir.x, 0, moveDir.y);
                transform.Translate(move, Space.World);
            }
        }

        
        private void OnMove(Vector2 dir) => moveDir = dir;
        private void OnLeftControl(bool pressed) => leftCtrlPressed = pressed;
    }
}
