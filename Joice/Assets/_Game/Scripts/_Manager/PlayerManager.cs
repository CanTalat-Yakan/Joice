using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    
    [SerializeField] internal MyCode.PlayerController Controller;
    [SerializeField] internal CharacterController CharacterController;
    [SerializeField] internal Animator PlayerAnimator;
    [SerializeField] internal GameObject PlayerArmature;
    [SerializeField] internal Transform PlayerCameraRoot;
    internal Vector3 PlayerPos { get => PlayerArmature.transform.position; }

    void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    internal void SetTransform(Transform _transform)
    {
        PlayerArmature.transform.position = _transform.position;
        PlayerArmature.transform.rotation = _transform.rotation;
    }
}
