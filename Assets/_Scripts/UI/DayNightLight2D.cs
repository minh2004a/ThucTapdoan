using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightLight2D : MonoBehaviour
{
    [SerializeField] Light2D globalLight;   // kéo Global Light vô, hoặc để trống cho auto
    TimeManager timeManager;

    [Header("Độ sáng")]
    [SerializeField] float minIntensity = 0.15f;  // tối nhất
    [SerializeField] float maxIntensity = 1.0f;   // sáng nhất

    [Header("Màu")]
    [SerializeField] Color dayColor = Color.white;
    [SerializeField] Color nightColor = new Color(0.3f, 0.35f, 0.6f);

    void Awake()
    {
        if (!globalLight)
            globalLight = GetComponent<Light2D>();
    }

    void Start()
    {
        // Tự kiếm TimeManager bên scene chung (Persistent)
        timeManager = FindObjectOfType<TimeManager>();
        if (!timeManager)
            Debug.LogError("DayNightLight2D: Không tìm thấy TimeManager trong scene!");
    }

    void Update()
    {
        if (!timeManager || !globalLight) return;

        // 0..1 theo giờ trong ngày
        float t = timeManager.GetDayLight01();

        // Độ sáng
        globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // Màu
        globalLight.color = Color.Lerp(nightColor, dayColor, t);
    }
}
