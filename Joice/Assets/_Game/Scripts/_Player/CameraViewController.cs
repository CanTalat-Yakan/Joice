using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraViewController : MonoBehaviour
{
    CameraAttributes m_attributes { get => CameraManager.Instance.Attributes; }

    InputManager m_input;
    CinemachineVirtualCamera m_vcam;
    Cinemachine3rdPersonFollow m_followComponent;
    float m_viewDist = 0;
    float m_tmpViewDist = 0;
    Vector3 m_tmpDamp;


    void Start()
    {
        m_tmpViewDist = m_attributes.StartViewDistance;
        m_input = InputManager.Instance;
        m_vcam = CameraManager.Instance.Virtual;

        m_followComponent = m_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_tmpDamp = m_followComponent.Damping;
    }

    void LateUpdate()
    {
        if (!(m_input is null || GameManager.Instance.LOCKED))
        {
            m_viewDist.Lerp(
                m_tmpViewDist -= m_input.zoom * m_attributes.DistanceChangeSpeed,
                10);
        }

        m_tmpViewDist.Clamp(0, m_attributes.MaxCameraDistance);
        m_viewDist.Clamp(0.01f, m_attributes.MaxCameraDistance);
        m_viewDist.Round();

        m_vcam.m_Lens.FieldOfView = UtilityManager.Map(m_viewDist,
            Mathf.Min(
                m_attributes.DistanceChangeTreshold,
                m_attributes.MaxCameraDistance), 0,
            m_attributes.ThirdPersonFieldOfView, m_attributes.FirsPersonFieldOfView);

        m_followComponent.CameraDistance = m_viewDist;
        m_followComponent.CameraRadius = m_attributes.CameraRadius;

        if (m_tmpViewDist < 1)
        {
            m_viewDist = 0;
            m_followComponent.Damping = Vector3.zero;
            m_followComponent.CameraSide = 0.5f;
            m_followComponent.VerticalArmLength = 0.4f;
        }
        if (m_tmpViewDist >= 1)
        {
            m_followComponent.Damping = m_tmpDamp;
            m_followComponent.CameraSide = 0.88f;
            m_followComponent.VerticalArmLength = 0.14f;
        }
    }
}
