using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Speed")]
    public float dayDurationSeconds = 60f;

    [Header("Sun colours")]
    public Gradient sunColor;
    public Gradient ambientColor;

    Light sun;
    float timeOfDay = 0.25f; // start at sunrise

    void Awake()
    {
        sun = GetComponent<Light>();
        sun.type      = LightType.Directional;
        sun.shadows   = LightShadows.Soft;
        sun.shadowStrength = 0.8f;

        if (sunColor == null || sunColor.colorKeys.Length == 0)
            sunColor = BuildSunGradient();

        if (ambientColor == null || ambientColor.colorKeys.Length == 0)
            ambientColor = BuildAmbientGradient();
    }

    void Update()
    {
        timeOfDay = (timeOfDay + Time.deltaTime / dayDurationSeconds) % 1f;

        // rotate sun: 0=midnight top, 0.25=sunrise, 0.5=noon, 0.75=sunset
        float angle = timeOfDay * 360f - 90f;
        transform.rotation = Quaternion.Euler(angle, -30f, 0f);

        sun.color     = sunColor.Evaluate(timeOfDay);
        sun.intensity = Mathf.Clamp01(Mathf.Sin(timeOfDay * Mathf.PI)); // 0 at night

        RenderSettings.ambientLight = ambientColor.Evaluate(timeOfDay);
    }

    Gradient BuildSunGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.0f, 0.0f, 0.1f), 0.00f), // midnight
                new GradientColorKey(new Color(1.0f, 0.4f, 0.1f), 0.22f), // sunrise
                new GradientColorKey(new Color(1.0f, 0.95f, 0.8f), 0.30f), // morning
                new GradientColorKey(new Color(1.0f, 1.0f, 1.0f), 0.50f), // noon
                new GradientColorKey(new Color(1.0f, 0.6f, 0.2f), 0.75f), // sunset
                new GradientColorKey(new Color(0.0f, 0.0f, 0.1f), 1.00f), // midnight
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        return g;
    }

    Gradient BuildAmbientGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.02f, 0.02f, 0.08f), 0.00f), // midnight
                new GradientColorKey(new Color(0.4f,  0.3f,  0.5f),  0.22f), // sunrise
                new GradientColorKey(new Color(0.5f,  0.6f,  0.7f),  0.50f), // noon
                new GradientColorKey(new Color(0.4f,  0.25f, 0.2f),  0.75f), // sunset
                new GradientColorKey(new Color(0.02f, 0.02f, 0.08f), 1.00f), // midnight
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        return g;
    }
}
