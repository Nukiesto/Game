using System.Collections.Generic;
using UnityEngine;

namespace Game.Lighting
{
    public class AmbientLightController : MonoBehaviour
    {
        [Header("Параметры")]
        [Range(0, 1)] public float lightLevel = 0f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AnimationCurve lightLevelTimeDefault;
        
        private Color _color;
        
        private void Awake()
        {
            _color = Color.white;
            UpdateLight();
        }

        private void UpdateLight()
        {
            _color.a = lightLevel;
            spriteRenderer.color = _color;
        }
        private void SetLight(float a)
        {
            lightLevel = a;
            UpdateLight();
        }
        public void OnValidate()
        {
            UpdateLight();
        }

        public void SetTime(float hour)
        {
            if (hour < 0 || hour > 24) return;
            
            var levelLight = lightLevelTimeDefault.Evaluate(hour);
            levelLight = levelLight > 1 ? 1 : levelLight < 0 ? 0 : levelLight;

            SetLight(levelLight);
            UpdateLight();
        }
    }
}
