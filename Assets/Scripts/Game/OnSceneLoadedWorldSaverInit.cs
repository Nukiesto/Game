using System;
using Singleton;
using UnityEngine;

namespace Game
{
    public class OnSceneLoadedWorldSaverInit : MonoBehaviour
    {
        private void Start()
        {
            Toolbox.Instance.mWorldSaver.InitWorld();
        }
    }
}