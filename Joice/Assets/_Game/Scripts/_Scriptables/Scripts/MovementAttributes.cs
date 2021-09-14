using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Movement Attributes", fileName = "Movement Info", order = 3)]
public class MovementAttributes : ScriptableObject
{
    [Header("Controller")]
    public float MaxSlope = 45;
    public float StepOffset = 0.25f;
    public float Radius = 0.28f;


    [Header("Player")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;

    [Space(10)]
    public float JumpTimeout = 0.50f, FallTimeout = 0.15f, JumpWhileFallingTolerance = 0.15f;


    [Header("Player Grounded")]
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;


    [Header("Camera")]
    public float CameraAngleOverride = 0.0f;
    public float TopClamp = 70.0f; 
    public float BottomClamp = -30.0f;
}
