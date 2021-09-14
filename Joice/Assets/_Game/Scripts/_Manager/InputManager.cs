using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] internal StarterAssetsInputs Input;

    
    void Awake()
    {
        if (Instance is null)
            Instance = this;
    }
}
