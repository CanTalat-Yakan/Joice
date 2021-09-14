using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations;

[Serializable]
public class MountController
{
    [SerializeField] ParentConstraint m_parent;
    [SerializeField] MovementAttributes m_mountAttributes;
    [SerializeField] GameObject m_mountArmature;
    [SerializeField] Animator m_mountAnimator;
    [SerializeField] SkinnedMeshRenderer m_bodyRenderer;
    [SerializeField] SkinnedMeshRenderer m_beltRenderer;

    BaseController m_controller;
    MovementAttributes m_playerAttributes;
    bool m_inProcess = false, m_previouslyMounted = false, m_mounted = false;
    float m_targetHorizontal, m_targetVertical, horizontalOffset;
    float m_slopeRotation;
    int m_currentState = 1;

    int m_animIDHorizontal, m_animIDVertical;
    int m_animIDGrounded, m_animIDJump, m_animIDFreeFall;
    int m_animIDMount;

    bool m_hasAnimator { get => m_mountAnimator; }



    internal void TryAssign(BaseController _controller)
    {
        if (m_controller is null)
            m_controller = _controller;

        m_playerAttributes = m_controller.m_Attributes;

        BaseController.SetAnimatorID(ref m_animIDHorizontal, "Horizontal");
        BaseController.SetAnimatorID(ref m_animIDVertical, "Vertical");
        BaseController.SetAnimatorID(ref m_animIDGrounded, "Grounded");
        BaseController.SetAnimatorID(ref m_animIDFreeFall, "FreeFall");
        BaseController.SetAnimatorID(ref m_animIDJump, "Jump");
        BaseController.SetAnimatorID(ref m_animIDMount, "Mount");
    }
    internal void OnUpdate()
    {
        if (m_controller.m_Input.mount != m_previouslyMounted)
            if (!m_inProcess)
            {
                m_controller.StopCoroutine(OnMounting());
                m_controller.StartCoroutine(OnMounting());
            }
        m_previouslyMounted = (m_controller.m_Input.mount = false);

        SetRootPos();
        SetMovementInfo();

        m_parent.weight = Mathf.Lerp(
            m_parent.weight,
            m_mounted
                ? 1
                : 0,
            Time.deltaTime * 10);
        m_parent.rotationAtRest = Vector3.zero;
        m_parent.translationAtRest = Vector3.zero;

        m_bodyRenderer.material.SetFloat("_strength", Mathf.Lerp(m_bodyRenderer.material.GetFloat("_strength"),
            m_mounted
                ? 1
                : 0,
            Time.deltaTime * 6));

        m_beltRenderer.material.SetFloat("_strength", Mathf.Lerp(m_beltRenderer.material.GetFloat("_strength"),
            m_mounted
                ? 1
                : 0,
            Time.deltaTime * 6));
    }

    IEnumerator OnMounting()
    {
        m_mounted = !m_mounted;
        m_inProcess = true;
        m_controller.LockMovement = true;

        if (m_mounted)
            m_mountArmature.SetActive(true);
        m_controller.m_Animator.SetBool(m_animIDMount, m_mounted);
        m_controller.m_Attributes = m_mounted
            ? m_mountAttributes
            : m_playerAttributes;

        yield return new WaitForSeconds(1);

        m_controller.LockMovement = false;

        yield return new WaitForSeconds(0.5f);

        m_inProcess = false;

        if (!m_mounted)
            m_mountArmature.SetActive(false);

        yield return null;
    }

    void SetRootPos()
    {
        Vector3 rootPos = m_controller.m_CameraRoot.localPosition;

        rootPos.y.Lerp(
            m_mounted ? 2.05f : 1.375f,
            m_controller.m_Attributes.SpeedChangeRate);

        m_controller.m_CameraRoot.localPosition = rootPos;
    }
    void SetMovementInfo()
    {
        float rotInput = m_controller.m_Input.move != Vector2.zero
            ? Vector3.Dot(
                m_controller.m_TargetDirection,
                m_controller.transform.right)
            : 0;

        horizontalOffset.Lerp(
            rotInput > 0 ? 1 : -1,
            m_controller.m_Attributes.SpeedChangeRate);

        if (Mathf.Abs(rotInput) < 0.1f)
            horizontalOffset = 0;

        m_targetHorizontal.Lerp(
            horizontalOffset + (m_controller.m_Input.move.x * m_controller.m_Input.move.y * 0.5f),
            m_controller.m_Attributes.SpeedChangeRate);
        m_targetHorizontal.Round();

        m_targetVertical.Lerp(
            m_controller.m_Speed.Remap(
                0, m_controller.m_Attributes.SprintSpeed,
                0, 5),
            m_controller.m_Attributes.SpeedChangeRate);
        m_targetVertical.Round();

        m_slopeRotation = m_mounted
            ? m_controller.m_Slope
            : 0;

        m_mountArmature.transform.localRotation = Quaternion.Euler(
            m_slopeRotation * 45,
            0, 0);
        m_mountArmature.transform.localPosition = Vector3.up * Mathf.Min(0, m_slopeRotation * -0.5f);
    }


    internal void OnMoving()
    {
        if (!m_controller.LockMovement && m_mounted)
        {
            m_mountAnimator.SetFloat(m_animIDHorizontal, m_targetHorizontal);
            m_mountAnimator.SetFloat(m_animIDVertical, m_targetVertical);
        }
        else
        {
            m_mountAnimator.SetFloat(m_animIDHorizontal, 0);
            m_mountAnimator.SetFloat(m_animIDVertical, 0);
        }
    }
    internal void OnFalling()
    {
        {
            m_mountAnimator.SetBool(m_animIDFreeFall, true);
        }
    }
    internal void OnGrounded()
    {
        {
            m_mountAnimator.SetBool(m_animIDGrounded, true);
        }
    }
    internal void OnJumping()
    {
        {
            m_mountAnimator.SetBool(m_animIDJump, true);
            m_mountAnimator.SetBool(m_animIDGrounded, false);
        }
    }
    internal void OnJump() { }
    internal void OnGround()
    {
        {
            m_mountAnimator.SetBool(m_animIDFreeFall, false);
            m_mountAnimator.SetBool(m_animIDJump, false);
            m_mountAnimator.SetBool(m_animIDGrounded, true);
        }
    }
}
