using System;
using Light2D;
using UnityEngine;

namespace Game.Lighting
{
    public class LightBlock : MonoBehaviour
    {
        [SerializeField] private LightSprite lightSprite;
        private float _alpha;
        public void SetPos(Vector3Int pos, ChunkUnit chunk)
        {
            transform.position = chunk.transform.position + pos;
        }

        private void OnEnable()
        {
            var color = lightSprite.Color;
            color.a = _alpha;
            lightSprite.Color = color;
        }

        public void SetColor(Color color)
        {
            _alpha = color.a;
            lightSprite.Color = color;
            //lightSprite.UpdateMeshData();
        }
        public void SetSize(float size)
        {
            var scale = new Vector3(size, size);
            transform.localScale = scale;
        }

        public void DestroyLight()
        {
            gameObject.SetActive(false);
        }
    }
}