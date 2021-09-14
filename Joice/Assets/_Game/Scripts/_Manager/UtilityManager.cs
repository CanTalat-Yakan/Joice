using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UtilityManager : MonoBehaviour
{
    public static UtilityManager Instance { get; private set; }
    [SerializeField] LayerMask IgnoreLayer;

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

    void Update()
    {
        if (InputManager.Instance)
            InputManager.Instance.enabled = !GameManager.Instance.LOCKED;
    }

    internal RaycastHit HitRayCast(float _maxDistance, Ray? _ray = null)
    {
        RaycastHit hit;

        Physics.Raycast(
            _ray is null
                ? CameraManager.Instance.Main.ViewportPointToRay(new Vector2(0.5f, 0.5f))
                : _ray.Value,
            out hit,
            _maxDistance,
            ~IgnoreLayer);

        return hit;
    }
    internal bool BoolRayCast(float _maxDistance, Ray? _ray = null)
    {
        return Physics.Raycast(
            _ray is null
                ? CameraManager.Instance.Main.ViewportPointToRay(new Vector2(0.5f, 0.5f))
                : _ray.Value,
            _maxDistance,
            ~IgnoreLayer);
    }
    internal static float Map(float _oldValue, float _oldMin, float _oldMax, float _newMin, float _newMax)
    {
        float oldRange = _oldMax - _oldMin;
        float newRange = _newMax - _newMin;
        float newValue = ((_oldValue - _oldMin) * newRange / oldRange) + _newMin;

        return Mathf.Clamp(newValue, _newMin, _newMax);
    }
}
public static class ExtensionMethods
{
    public static float Remap(this float _value, float _oldMin, float _oldMax, float _newMin, float _newMax)
    {
        return (_value - _oldMin) / (_oldMax - _oldMin) * (_newMax - _newMin) + _newMin;
    }
    public static float Lerp(ref this float _value, float _target, float _time = 1)
    {
        return _value = Mathf.Lerp(
            _value,
            _target,
            Time.deltaTime * _time);
    }
    public static float Round(ref this float _value, float _digits = 3)
    {
        return _value = Mathf.Round(_value * Mathf.Pow(10, _digits)) / Mathf.Pow(10, _digits);
    }
    public static float Clamp(ref this float _value, float _min = 0, float _max = 1)
    {
        return _value = Mathf.Clamp(_value, _min, _max);
    }
}
