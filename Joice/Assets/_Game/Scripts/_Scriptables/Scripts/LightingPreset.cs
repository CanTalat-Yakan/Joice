using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 5)]
public class LightingPreset : ScriptableObject
{
    [Header("DayTime")]
    public AnimationCurve DayTimeFlow;
    [Header("Colors")]
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;


    [Header("Light Attributes")]
    public float DawnDegree = 20;
    public float DayLightIntensity = 1.3f, NightLightIntensity = 0.4f;

    [Header("Sky Attributes")]
    public Color SkyColor;
    public float SunSize = 0.04f, MoonSize = 0.024f;
    public float NightTimeSkyExposure = 0.1f;
    public float EnvironmentSkyLightIntensitiy = 0.75f;
}
