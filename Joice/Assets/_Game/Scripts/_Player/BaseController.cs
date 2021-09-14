using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    internal bool LockMovement { get => m_lockMovement || GameManager.Instance.LOCKED; set => m_lockMovement = value; }
    internal bool LockCamera { get => m_lockCamera || GameManager.Instance.LOCKED; set => m_lockCamera = value; }

    [Header("References")]
    [SerializeField] internal MovementAttributes m_Attributes;
    internal Animator m_Animator;
    internal CharacterController m_CharController;
    internal InputManager m_Input;
    internal Transform m_Camera;
    internal Transform m_CameraRoot;
    bool m_lockMovement = false, m_lockCamera = false;

    // cinemachine
    float m_cinemachineTargetYaw, m_cinemachineTargetPitch;

    // animation IDs
    int m_animIDSpeed;
    int m_animIDGrounded;
    int m_animIDJump;
    int m_animIDFreeFall;
    int m_animIDMotionSpeed;
    int m_animIDSlope;

    // player
    internal float m_Speed, m_Slope;
    internal Vector3 m_TargetDirection;
    internal RaycastHit m_PointHit;
    bool m_grounded = true, m_previouslyGrounded = false;
    bool m_jumping = false;
    float m_animationBlend;
    float m_targetRotation = 0.0f;
    float m_rotationVelocity, m_verticalVelocity, m_terminalVelocity = 53.0f;
    const float m_inputThreshold = 0.01f;

    // timeout deltatime
    float m_jumpTimeoutDelta, m_fallTimeoutDelta, m_jumpWhileFallingTolerance;


    bool m_hasAnimator { get => m_Animator; }
    float m_inputMagnitude { get => m_Input.analogMovement ? m_Input.move.magnitude : 1f; }


    internal void OnStart()
    {
        // reset our timeouts on start
        m_jumpTimeoutDelta = m_Attributes.JumpTimeout;
        m_fallTimeoutDelta = m_Attributes.FallTimeout;

        m_Animator = PlayerManager.Instance.PlayerAnimator;
        m_CharController = PlayerManager.Instance.CharacterController;
        m_Input = InputManager.Instance;
        m_Camera = CameraManager.Instance.Main.transform;
        m_CameraRoot = PlayerManager.Instance.PlayerCameraRoot;

        AssignAnimationIDs();
    }
    internal void OnUpdate()
    {
        m_CharController.slopeLimit = m_Attributes.MaxSlope;
        m_CharController.stepOffset = m_Attributes.StepOffset;
        m_CharController.radius = m_Attributes.Radius;

        Cursor.visible &= GameManager.Instance.LOCKED;
        Cursor.lockState &= Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    internal virtual void AssignAnimationIDs()
    {
        SetAnimatorID(ref m_animIDSpeed, "Speed");
        SetAnimatorID(ref m_animIDGrounded, "Grounded");
        SetAnimatorID(ref m_animIDJump, "Jump");
        SetAnimatorID(ref m_animIDFreeFall, "FreeFall");
        SetAnimatorID(ref m_animIDMotionSpeed, "MotionSpeed");
        SetAnimatorID(ref m_animIDSlope, "Slope");
    }

    internal virtual void OnMoving()
    {
        if (m_hasAnimator)
        {
            m_Animator.SetFloat(m_animIDSpeed, m_animationBlend);
            m_Animator.SetFloat(m_animIDMotionSpeed, m_inputMagnitude);
            m_Animator.SetFloat(m_animIDSlope, -m_Slope);
        }
    }
    internal virtual void OnGrounded()
    {
        if (m_hasAnimator)
        {
            m_Animator.SetFloat(m_animIDSpeed, m_animationBlend);
            m_Animator.SetFloat(m_animIDMotionSpeed, m_inputMagnitude);
            m_Animator.SetBool(m_animIDGrounded, true);
            m_Animator.SetBool(m_animIDFreeFall, false);
        }
    }
    internal virtual void OnJumping()
    {
        if (m_hasAnimator)
        {
            m_Animator.SetBool(m_animIDJump, true);
            m_Animator.SetBool(m_animIDFreeFall, false);
            m_Animator.SetBool(m_animIDGrounded, false);
        }
    }
    internal virtual void OnJump() { }
    internal virtual void OnFalling()
    {
        if (m_hasAnimator)
        {
            m_Animator.SetBool(m_animIDJump, false);
            m_Animator.SetBool(m_animIDFreeFall, true);
            m_Animator.SetBool(m_animIDGrounded, false);
        }
    }
    internal virtual void OnGround()
    {
        if (m_hasAnimator)
        {
            m_Animator.SetBool(m_animIDJump, false);
            m_Animator.SetBool(m_animIDJump, false);
            m_Animator.SetBool(m_animIDFreeFall, false);
        }
    }


    internal void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 originPosition = new Vector3(
            transform.position.x,
            transform.position.y - m_Attributes.GroundedOffset,
            transform.position.z);

        m_PointHit = UtilityManager.Instance.HitRayCast(
            1.0f,
            new Ray(
                originPosition,
                -transform.up));

        m_Slope.Lerp(
            Vector3.Dot(
                m_PointHit.normal,
                transform.forward),
            m_Attributes.SpeedChangeRate);
        m_Slope.Round();
        if (m_jumping)
            m_Slope = 0;

        m_grounded = Physics.CheckSphere(
            originPosition,
            m_Attributes.GroundedRadius,
            m_Attributes.GroundLayers,
            QueryTriggerInteraction.Ignore);

        if (m_grounded)
            OnGrounded();

        if (m_grounded != m_previouslyGrounded)
        {
            if (m_grounded)
                OnGround();
            else
                OnJump();
        }
        m_previouslyGrounded = m_grounded;

    }
    internal void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (m_Input.look.sqrMagnitude >= m_inputThreshold && !LockCamera)
        {
            m_cinemachineTargetYaw += m_Input.look.x * Time.deltaTime;
            m_cinemachineTargetPitch += m_Input.look.y * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        m_cinemachineTargetYaw = ClampAngle(
            m_cinemachineTargetYaw,
            float.MinValue,
            float.MaxValue);
        m_cinemachineTargetPitch = ClampAngle(
            m_cinemachineTargetPitch,
            m_Attributes.BottomClamp,
            m_Attributes.TopClamp);

        // Cinemachine will follow this target
        m_CameraRoot.rotation = Quaternion.Euler(
            m_cinemachineTargetPitch + m_Attributes.CameraAngleOverride,
            m_cinemachineTargetYaw,
            0.0f);
    }
    internal void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = m_Input.sprint
            ? m_Attributes.SprintSpeed
            : m_Attributes.MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (m_Input.move == Vector2.zero || LockMovement)
            targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(
            m_CharController.velocity.x,
            0.0f,
            m_CharController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            m_Speed = Mathf.Lerp(
                currentHorizontalSpeed,
                targetSpeed * m_inputMagnitude,
                Time.deltaTime * m_Attributes.SpeedChangeRate);

            // round speed to 3 decimal places
            m_Speed = Mathf.Round(m_Speed * 1000f) / 1000f;
        }
        else
        {
            m_Speed = targetSpeed;
        }
        m_animationBlend.Lerp(
            targetSpeed,
            m_Attributes.SpeedChangeRate);

        // normalise input direction
        Vector3 inputDirection = new Vector3(
            m_Input.move.x,
            0.0f,
            m_Input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (m_Input.move != Vector2.zero && !LockMovement)
        {
            m_targetRotation = Mathf.Atan2(
                inputDirection.x,
                inputDirection.z) * Mathf.Rad2Deg + m_Camera.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                m_targetRotation,
                ref m_rotationVelocity,
                m_Attributes.RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(
                0.0f,
                rotation,
                0.0f);
        }


        m_TargetDirection = Quaternion.Euler(
            0.0f,
            m_targetRotation,
            0.0f) * Vector3.forward;

        // project surface normal on target direction with sum of both
        Vector3 slopeDirection = Vector3.ProjectOnPlane(m_TargetDirection, m_PointHit.normal);
        m_TargetDirection = slopeDirection + m_TargetDirection;

        // move the player
        m_CharController.Move(m_TargetDirection.normalized * (m_Speed * Time.deltaTime) + Vector3.up * m_verticalVelocity * Time.deltaTime);

        if (m_Speed != 0 && m_grounded)
            OnMoving();
    }

    internal void JumpAndGravity()
    {
        if (m_grounded)
        {
            // reset the fall timeout timer
            m_fallTimeoutDelta = m_Attributes.FallTimeout;

            // reset tolerance of jumping while falling
            m_jumpWhileFallingTolerance = m_Attributes.JumpWhileFallingTolerance;

            // jump timeout
            if (m_jumpTimeoutDelta >= 0.0f)
                m_jumpTimeoutDelta -= Time.deltaTime;

            // stop our velocity dropping infinitely when grounded
            if (m_verticalVelocity < 0.0f)
                m_verticalVelocity = -2f;

            // Jump
            if (!LockMovement && m_Input.jump && m_jumpTimeoutDelta <= 0.0f)
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                m_verticalVelocity = Mathf.Sqrt(m_Attributes.JumpHeight * -2f * m_Attributes.Gravity);
        }
        else
        {
            // reset the jump timeout timer
            m_jumpTimeoutDelta = m_Attributes.JumpTimeout;

            // jump tolerance timer
            if (m_jumpWhileFallingTolerance >= 0.0f)
                m_jumpWhileFallingTolerance -= Time.deltaTime;

            // fall timeout
            if (!m_jumping)
            {
                if (m_fallTimeoutDelta >= 0.0f)
                    m_fallTimeoutDelta -= Time.deltaTime;
                else
                    OnFalling();
            }

            // Jump
            if (!LockMovement && m_Input.jump && m_jumpWhileFallingTolerance >= 0.0f)
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                m_verticalVelocity = Mathf.Sqrt(m_Attributes.JumpHeight * -2f * m_Attributes.Gravity);

            // if we are not grounded, do not jump
            m_Input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (m_verticalVelocity < m_terminalVelocity)
            m_verticalVelocity += m_Attributes.Gravity * Time.deltaTime;

        // stop movement due collider above head and 
        if (m_jumping && Mathf.Approximately(m_CharController.velocity.y, 0))
            m_verticalVelocity = 0;

        // call OnJumping when velocity is abvove zero
        if (m_jumping = (m_verticalVelocity > 0))
            OnJumping();
    }

    internal static float ClampAngle(float _angle, float _min, float _max)
    {
        if (_angle < -360f) _angle += 360f;
        if (_angle > 360f) _angle -= 360f;
        return Mathf.Clamp(_angle, _min, _max);
    }
    internal static void SetAnimatorID(ref int _id, string _param)
    {
        _id = Animator.StringToHash(_param);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + m_TargetDirection.normalized);
    }
}
