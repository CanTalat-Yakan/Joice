using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class LightingSettingsManager : MonoBehaviour
{
    public static LightingSettingsManager Instance { get; private set; }

    internal double m_TimeOfDay;
    internal float m_DirectionalLightAlpha;
    [SerializeField] Material m_skyBox;
    [SerializeField] Volume m_volume;
    [SerializeField] Light m_sunLight;
    [SerializeField] Light m_moonLight;
    [SerializeField] Light m_skyLight;
    [SerializeField] LightingPreset m_preset;
    [SerializeField, Range(0, 24)] double m_timeOfDayOffset;
    ColorAdjustments m_colorAdjustments;


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

    void Start()
    {
        RenderSettings.skybox = m_skyBox;
        RenderSettings.fogDensity = 0.0005f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;

        m_volume.profile.TryGet(out m_colorAdjustments);
    }

    void Update()
    {
        if (m_preset is null)
            return;

        if (Application.isPlaying)
        {
            // m_timeOfDay += Time.deltaTime;
            m_TimeOfDay = System.DateTime.Now.TimeOfDay.TotalHours + m_timeOfDayOffset;
            m_TimeOfDay %= 24;
            UpdateLighting((float)(m_TimeOfDay / 24f));
        }
        else
            UpdateLighting((float)(m_TimeOfDay / 24f));
    }

    void UpdateLighting(float _timePercent)
    {
        _timePercent = m_preset.DayTimeFlow.Evaluate(_timePercent);
        RenderSettings.ambientLight = m_preset.AmbientColor.Evaluate(_timePercent);
        RenderSettings.fogColor = m_preset.FogColor.Evaluate(_timePercent);

        if (m_sunLight != null && m_moonLight != null)
        {
            m_DirectionalLightAlpha = Vector3.Dot(Vector3.up, -m_sunLight.transform.forward);
            float alphaDegree = 90 * m_DirectionalLightAlpha;

            m_sunLight.color = m_preset.DirectionalColor.Evaluate(_timePercent);

            m_sunLight.transform.localRotation = Quaternion.Euler(new Vector3((_timePercent * 360f) - 90f, 170f, 0));

            m_sunLight.intensity = UtilityManager.Map(alphaDegree, 0, m_preset.DawnDegree, 0, m_preset.DayLightIntensity);
            m_moonLight.intensity = UtilityManager.Map(alphaDegree, 0, -m_preset.DawnDegree, 0, m_preset.NightLightIntensity);
            m_skyLight.intensity = m_preset.NightTimeSkyExposure;
            
            m_skyBox.SetFloat("_Exposure", UtilityManager.Map(alphaDegree, -m_preset.DawnDegree, m_preset.DawnDegree, m_preset.NightTimeSkyExposure, m_preset.DayLightIntensity));
            m_skyBox.SetFloat("_SunSize", UtilityManager.Map(alphaDegree, -m_preset.DawnDegree, m_preset.DawnDegree, m_preset.MoonSize, m_preset.SunSize));
            m_skyBox.SetColor("_SkyTint", m_preset.SkyColor);

            RenderSettings.sun = alphaDegree > 0 ? m_sunLight : m_moonLight;
            RenderSettings.ambientIntensity = m_preset.EnvironmentSkyLightIntensitiy;

            m_colorAdjustments.postExposure.value = UtilityManager.Map(alphaDegree, -m_preset.DawnDegree, m_preset.DawnDegree, 0, m_preset.DayLightIntensity);
        }
    }
}
