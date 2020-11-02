using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game
{
    public class RandomBack : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] sprites;
        
        private void Start()
        {
            var n = Random.Range(0, sprites.Length);
            image.sprite = sprites[n];
        }
    }
}