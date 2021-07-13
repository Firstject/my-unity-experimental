using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DayNight
{
    public class LightingManager : MonoBehaviour
    {
        [SerializeField] private Light DirectionalLight;
        [SerializeField] private LightingPreset Preset;

        [SerializeField, Range(0, 1)] private float TimeOfDay;

        private float timeRate = 0.1f;

        private void Update()
        {
            TimeIncrement();
            UpdateLighting(TimeOfDay / 1f);
        }

        private void OnValidate()
        {
            UpdateDirectionalLight();
        }

        private void TimeIncrement()
        {
            if (Application.isPlaying)
            {
                TimeOfDay += Time.deltaTime * timeRate;
                TimeOfDay %= 1;
            }
        }

        private void UpdateLighting(float timePercent)
        {
            if (Preset == null)
            {
                return;
            }

            RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

            if (DirectionalLight != null)
            {
                DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
                DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -90f, -90f));
            }
        }

        void UpdateDirectionalLight()
        {
            // Try to find a directional light to use if we haven't set one
            if (DirectionalLight != null)
            {
                return;
            }

            // Search for lighting tab sun
            if (RenderSettings.sun != null)
            {
                DirectionalLight = RenderSettings.sun;
                return;
            }

            // Search scene for light that fits criteria (directional)
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}
