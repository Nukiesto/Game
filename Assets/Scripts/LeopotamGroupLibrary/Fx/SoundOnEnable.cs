// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.Fx
{
    /// <summary>
    /// Setup FX parameters on enable.
    /// </summary>
    public sealed class SoundOnEnable : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _sound;

        [SerializeField]
        private SoundFxChannel _channel = SoundFxChannel.First;

        /// <summary>
        /// Should new FX force interrupts FX at channel or not.
        /// </summary>
        [SerializeField]
        private bool _isInterrupt;

        private void OnEnable()
        {
            Service<SoundManager>.Get().PlayFx(_sound, _channel, _isInterrupt);
        }
    }
}