using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    [SerializeField] internal Camera Main;
    [SerializeField] internal CinemachineVirtualCamera Virtual;
    [SerializeField] internal CameraAttributes Attributes;

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
}
