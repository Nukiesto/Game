// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using System.Collections;
using UnityEngine;

namespace LeopotamGroup.Fx
{
    /// <summary>
    /// Setup FX parameters on start.
    /// </summary>
    public sealed class SoundOnStart : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _sound;

        [SerializeField]
        private SoundFxChannel _channel = SoundFxChannel.First;

        /// <summary>
        /// Should new FX force interrupts FX at channel or not.
        /// </summary>
        public bool IsInterrupt;

        private IEnumerator Start()
        {
            yield return null;
            Service<SoundManager>.Get().PlayFx(_sound, _channel, IsInterrupt);
        }
    }
}