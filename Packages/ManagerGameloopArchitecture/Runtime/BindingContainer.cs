using ManagerInjection.Managers;
using NSubstitute;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManagerInjection
{
    public class BindingContainer
    {
        public enum BindingType
        {
            Prefab,
            Instance,
            Mock,
        }
        
        public IReadOnlyDictionary<Type, IManager> Managers => _managersRefByType;
        private Dictionary<Type, IManager> _managersRefByType;

        private Dictionary<IManager, IManager> _managersSources;
        
        protected event Action OnUnbindAll;

        private Transform m_transform;

        public BindingContainer(Transform transform = null)
        {
            _managersRefByType = new Dictionary<Type, IManager>();
            _managersSources = new Dictionary<IManager, IManager>();
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
        
        public T Bind<T>(MockBinder<T> binder)
            where T : class, IManager
        {
            T newManager = BindWithMockService<T>();

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
            if (!_managersSources.TryGetValue(managerPrefab, out IManager manager))
            {
                BaseManager baseManager = UnityEngine.Object.Instantiate(managerPrefab, m_transform);
                baseManager.name = managerPrefab.name;
                manager = baseManager;
                _managersSources.Add(managerPrefab, manager);
            }
            _managersRefByType.Add(typeof(T), manager);
            return manager as T;
        }

        public T BindWithInstance<T>(IManager managerInstance) where T : class, IManager
        {
            if (!CheckTypeKey(managerInstance, out T mockManager))
            {
                return mockManager;
            }

            if (!_managersSources.TryGetValue(managerInstance, out IManager manager))
            {
                _managersSources.Add(managerInstance, managerInstance);
                if (managerInstance is Component component && component.transform != m_transform)
                {
                    component.transform.SetParent(m_transform);
                }
                manager = managerInstance;
            }
            _managersRefByType.Add(typeof(T), manager);
            return manager as T;
        }

        public T BindWithMockService<T>() where T : class, IManager
        {
            if (_managersRefByType.ContainsKey(typeof(T)))
            {
                Debug.LogError($"The binding container already references the {typeof(T).Name} type.");
                return null;
            }

            T mockService = Substitute.For<T>();
            _managersRefByType.Add(typeof(T), mockService);
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
            if (_managersRefByType.TryGetValue(typeof(T), out IManager managerBinded))
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
