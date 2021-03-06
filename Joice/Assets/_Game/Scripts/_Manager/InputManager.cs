using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    void Awake()
    {
        if (Instance is null)
            Instance = this;
    }   

    [Header("Misc")]
    public bool debug;
    public bool enter;
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public float zoom;
    public bool mount;
    [Header("Movement Settings")]
    public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnDebug(InputValue value)
        {
            DebugInput(value.isPressed);
        }
        public void OnEnter(InputValue value)
        {
            EnterInput(value.isPressed);
        }

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnZoom(InputValue value)
        {
            ZoomInput(value.Get<Vector2>());
        }

        public void OnRide(InputValue value)
        {
            MountInput(value.isPressed);
        }
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


        public void DebugInput(bool newDebug)
        {
            if (newDebug)
                debug = !debug;
        }
        public void EnterInput(bool newEnter)
        {
                debug = newEnter;
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void ZoomInput(Vector2 newZoomState)
        {
            zoom = newZoomState.y;
        }

        public void MountInput(bool newRideState)
        {
            mount = newRideState;
        }

#if !UNITY_IOS || !UNITY_ANDROID

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

#endif

}
