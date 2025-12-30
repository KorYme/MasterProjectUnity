using System;
using ManagerInjection.Managers;
using UnityEngine;

namespace ManagerInjection
{
    [Serializable]
    public struct ManagerBinder<T, Y> where T : class, IManager where Y : BaseManager
    {
        [field: SerializeField] public BindingContainer.BindingType BindingType { get; private set; }
        
        [field: SerializeField] public Y Manager { get; private set; }
        
        [field: SerializeField] public SOManagerReference<T> ManagerReference { get; private set; }

        public void Unbind()
        {
            (ManagerReference as IManagerReferenceSetter<T>)?.UnsetManagerReference();
        }
    }
}