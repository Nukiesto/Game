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
    /// Setup music parameters on start.
    /// </summary>
    public sealed class MusicOnStart : MonoBehaviour
    {
        [SerializeField]
        private string _music;

        [SerializeField]
        private bool _isLooped = true;

        private IEnumerator Start()
        {
            yield return null;
            var sm = Service<SoundManager>.Get();
            if (sm.MusicVolume <= 0f)
            {
                sm.StopMusic();
            }
            sm.PlayMusic(_music, _isLooped);
        }
    }
}