using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace Krivodeling.UI.Effects
{
    public class UIBlur : MonoBehaviour
    {
        #region Variables
        private Material material;

        public Color color = Color.white;
        public float Intensity { get => _intensity; set => _intensity = Mathf.Clamp01(value); }
        [SerializeField] [Range(0f, 1f)] private float _intensity;
        [Range(0f, 1f)] public float multiplier = 0.15f;

        [System.Serializable] public class BlurChangedEvent : UnityEvent<float> { }

        public UnityEvent onBeginBlur, onEndBlur;
        public BlurChangedEvent onBlurChanged = new BlurChangedEvent();
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void OnValidate()
        {
            SetBlurInEditor();
        }

        private void SetBlurInEditor()
        {
            Material m = GetComponent<Image>().material;
            m.SetColor("_Color", color);
            m.SetFloat("_Intensity", Intensity);
            m.SetFloat("_Multiplier", multiplier);
        }
#endif
        #endregion

        #region Methods
        private void Start()
        {
            SetComponents();
            SetBlur(color, Intensity, multiplier);
        }

        private void SetComponents()
        {
            material = GetComponent<Image>().materialForRendering;
        }

        public void SetBlur(Color color, float intensity, float multiplier)
        {
            material.SetColor("_Color", color);
            material.SetFloat("_Intensity", intensity);
            material.SetFloat("_Multiplier", multiplier);
        }

        public void SetBlur(float value)
        {
            material.SetFloat("_Intensity", value);
        }

        public void BeginBlur(float speed)
        {
            StopAllCoroutines();
            StartCoroutine(BeginBlurCoroutine(speed));
        }

        private IEnumerator BeginBlurCoroutine(float speed)
        {
            onBeginBlur?.Invoke();

            while (Intensity < 1f)
            {
                Intensity += speed;
                SetBlur(Intensity);
                onBlurChanged.Invoke(Intensity);

                yield return null;
            }
        }

        public void EndBlur(float speed)
        {
            StopAllCoroutines();
            StartCoroutine(EndBlurCoroutine(speed));
        }

        private IEnumerator EndBlurCoroutine(float speed)
        {
            while (Intensity > 0f)
            {
                Intensity -= speed;
                SetBlur(Intensity);
                onBlurChanged.Invoke(Intensity);

                yield return null;
            }

            onEndBlur?.Invoke();
        }
        #endregion
    }
}
