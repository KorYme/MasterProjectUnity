using System;
using ManagerInjection.Managers;
using UnityEngine;

namespace ManagerInjection
{
    [Serializable]
    public struct MockBinder<T> where T : class, IManager
    {
        [field: SerializeField] public SOManagerReference<T> ManagerReference { get; private set; }

        public void Unbind()
        {
            (ManagerReference as IManagerReferenceSetter<T>)?.UnsetManagerReference();
        }
    }
}