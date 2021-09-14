using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Camera Attributes", fileName = "Camera Info", order = 4)]
public class CameraAttributes : ScriptableObject
{
    public float CameraRadius = 0.5f;
    public float FirsPersonFieldOfView = 60;
    public float ThirdPersonFieldOfView = 40;
    public float StartViewDistance = 4;
    public float DistanceChangeTreshold = 4;
    public float MaxCameraDistance = 8;
    public float DistanceChangeSpeed = 1;
}
