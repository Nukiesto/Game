using UnityEngine;
using UnityEngine.Serialization;

public class AmbientLightController : MonoBehaviour
{
    [Header("Параметры")]
    [Range(0.1f, 1)] public float lightLevel = 0.1f;

    [SerializeField] private Renderer renderer;
    
    private void Start()
    {
        UpdateLight();
    }
    public void UpdateLight()
    {
        renderer.material.SetFloat(2, lightLevel);
        Debug.Log(lightLevel + ":" + renderer.material.shader.GetPropertyName(2) + " : "+ renderer.material.GetFloat(2));
    }
    public void OnValidate()
    {
        UpdateLight();
    }
}
