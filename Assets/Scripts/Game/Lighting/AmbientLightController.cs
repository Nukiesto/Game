using UnityEngine;

public class AmbientLightController : MonoBehaviour
{
    [Header("Параметры")]
    [Range(0, 100)] public float lightLevel = 20;

    private SpriteRenderer spr;

    private Color color;
    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        color = spr.color;
        UpdateLight();
    }
    public void UpdateLight()
    {
        color.a = lightLevel / 100;
        spr.color = color;
    }
    public void OnValidate()
    {
        UpdateLight();
    }
}
