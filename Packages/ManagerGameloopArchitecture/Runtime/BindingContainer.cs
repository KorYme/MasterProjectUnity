using ManagerInjection.Managers;
using NSubstitute;
using System;
using System.Collections.Generic;
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
    
    public class BindingContainer
    {
        public enum BindingType
        {
            Prefab,
            Instance,
            Mock,
        }
        
        public IReadOnlyDictionary<Type, IManager> AllManagers => _allManagers;
        private Dictionary<Type, IManager> _allManagers;

        protected event Action OnUnbindAll;

        private Transform m_transform;

        public BindingContainer(Transform transform = null)
        {
            _allManagers = new Dictionary<Type, IManager>();
            m_transform = transform;
            OnUnbindAll = null;
        }

        public T Bind<T, Y>(ManagerBinder<T, Y> binder)
            where T : class, IManager 
            where Y : BaseManager
        {
            T newManager = binder.BindingType switch
            {
                BindingType.Prefab => BindWithPrefab<T>(binder.Manager),
                BindingType.Instance => BindWithInstance<T>(binder.Manager),
                BindingType.Mock => BindWithMockService<T>(),
                _ => null,
            };

            if (binder.ManagerReference is IManagerReferenceSetter<T> managerReferenceSetter)
            {
                managerReferenceSetter.SetManagerReference(newManager);
                OnUnbindAll += managerReferenceSetter.UnsetManagerReference;
            }
            return newManager;
        }

        public T BindWithPrefab<T>(BaseManager managerPrefab) where T : class, IManager
        {
            if (!CheckTypeKey(managerPrefab, out T mockManager))
            {
                return mockManager;
            }
            BaseManager managerInstance = UnityEngine.Object.Instantiate(managerPrefab, m_transform);
            _allManagers.Add(typeof(T), managerInstance);
            // Debug.Log($"{nameof(BindingContainer)} : {typeof(T).Name} has been binded with a prefab instance.");
            return managerInstance as T;
        }

        public T BindWithInstance<T>(IManager managerInstance) where T : class, IManager
        {
            if (!CheckTypeKey(managerInstance, out T mockManager))
            {
                return mockManager;
            }
            _allManagers.Add(typeof(T), managerInstance);
            if (managerInstance is MonoBehaviour monoBehaviour && monoBehaviour.transform != m_transform)
            {
                monoBehaviour.transform.SetParent(m_transform);
            }
            // Debug.Log($"{nameof(BindingContainer)} : {typeof(T).Name} has been binded with a simple instance.");
            return managerInstance as T;
        }

        public T BindWithMockService<T>() where T : class, IManager
        {
            if (_allManagers.ContainsKey(typeof(T)))
            {
                Debug.LogError($"The binding container already references the {typeof(T).Name} type.");
                return null;
            }

            T mockService = Substitute.For<T>();
            _allManagers.Add(typeof(T), mockService);
            Debug.Log($"{nameof(BindingContainer)} : {typeof(T).Name} has been binded with a mock instance.");
            return mockService;
        }

        private bool CheckTypeKey<T>(IManager managerInstance, out T mockManager) where T : class, IManager
        {
            if (managerInstance is null)
            {
                Debug.LogWarning($"The instance of type {typeof(IManager).Name} is null.");
                mockManager = BindWithMockService<T>();
                return false;
            }
            if (managerInstance is not T)
            {
                Debug.LogError($"The object type you gave is not a manager.");
                mockManager = BindWithMockService<T>();
                return false;
            }
            if (_allManagers.TryGetValue(typeof(T), out IManager managerBinded))
            {
                Debug.LogError($"The binding container already references the {typeof(T).Name} type.");
                mockManager = managerBinded as T;
                return false;
            }
            mockManager = null;
            return true;
        }

        public void UnbindAll()
        {
            OnUnbindAll?.Invoke();
            OnUnbindAll = null;
        }
    }
}
