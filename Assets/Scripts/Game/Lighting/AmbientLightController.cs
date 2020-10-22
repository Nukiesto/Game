using UnityEngine;
using UnityEngine.Serialization;

public class AmbientLightController : MonoBehaviour
{
    [Header("Параметры")]
    [Range(0, 100)] public float lightLevel = 20;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color _color;
    private void Start()
    {
        _color = Color.white;
        UpdateLight();
    }
    public void UpdateLight()
    {
        _color.a = lightLevel / 100;
        spriteRenderer.color = _color;
    }
    public void OnValidate()
    {
        UpdateLight();
    }
}
