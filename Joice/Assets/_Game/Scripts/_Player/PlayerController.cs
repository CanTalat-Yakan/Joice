using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode
{
    public class PlayerController : BaseController
    {
        [SerializeField] internal MountController m_Mount;


        void Start()
        {
            OnStart();

            m_Mount.TryAssign(this);
        }

        void Update()
        {
            OnUpdate();
            JumpAndGravity();
            GroundedCheck();
            Move();
            m_Mount.OnUpdate();
        }

        void LateUpdate()
        {
            CameraRotation();
        }

        internal override void OnMoving() { base.OnMoving(); m_Mount.OnMoving(); }
        internal override void OnFalling() { base.OnFalling(); m_Mount.OnFalling(); }
        internal override void OnGrounded() { base.OnGrounded(); m_Mount.OnGrounded(); }
        internal override void OnJumping() { base.OnJumping(); m_Mount.OnJumping(); }
        internal override void OnJump() { base.OnJump(); m_Mount.OnJump(); }
        internal override void OnGround() { base.OnGround(); m_Mount.OnGround(); }
    }
}
