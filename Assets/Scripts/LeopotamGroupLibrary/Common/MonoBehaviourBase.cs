// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.Common
{
    /// <summary>
    /// ������ ������ MonoBehaviour � ������������ ��������������.
    /// </summary>
    public abstract class MonoBehaviourBase : MonoBehaviour
    {
        /// <summary>
        /// ������������ ��������������, ���������� ������������������ � 2 ���� �� ��������� �� �����������.
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
        /// ���������� ������������ ��������������. �� ���� �������, ������������� ���, ��� ������ ��� ��������������� ���������� ������������������ � 2 ����.
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