// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.Common
{
    /// <summary>
    /// «амена класса MonoBehaviour с кешированием преобразований.
    /// </summary>
    public abstract class MonoBehaviourBase : MonoBehaviour
    {
        /// <summary>
        /// »справленное преобразование, увеличение производительности в 2 раза по сравнению со стандартным.
        /// </summary>
        /// <value>The transform.</value>
        public new Transform transform
        {
            get
            {
                if ((object)CachedTransform == null)
                {
                    CachedTransform = base.transform;
                }
                return CachedTransform;
            }
        }

        /// <summary>
        /// ¬нутреннее кэшированное преобразование. Ќе будь дураком, перезаписыва€ его, нет защиты дл€ дополнительного увеличени€ производительности в 2 раза.
        /// </summary>
        protected Transform CachedTransform;

        protected virtual void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = base.transform;
            }
        }
    }
}