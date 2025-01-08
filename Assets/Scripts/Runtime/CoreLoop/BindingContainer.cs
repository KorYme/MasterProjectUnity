using MasterProject.Debugging;
using MasterProject.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject
{
    public class BindingContainer
    {
        public IReadOnlyDictionary<Type, IService> AllServices => m_allServices;
        private Dictionary<Type, IService> m_allServices;
        public IEnumerable<IService> ServicesUpdateOrder => m_servicesUpdateOrder;
        private LinkedList<IService> m_servicesUpdateOrder;
        public IEnumerable<IService> ServicesLateUpdateOrder => m_servicesLateUpdateOrder;
        private LinkedList<IService> m_servicesLateUpdateOrder;

        private Transform m_transform;

        public BindingContainer(Transform transform = null)
        {
            m_allServices = new Dictionary<Type, IService>();
            m_servicesUpdateOrder = new LinkedList<IService>();
            m_servicesLateUpdateOrder = new LinkedList<IService>();
            m_transform = transform;
        }

        public void SetupOrders(IReadOnlyList<Type> updateOrder, IReadOnlyList<Type> lateUpdateOrder)
        {
            foreach (Type type in updateOrder)
            {
                if (AllServices.TryGetValue(type, out IService service))
                {
                    m_servicesUpdateOrder.AddLast(service);
                }
            }
            foreach (Type type in lateUpdateOrder)
            {
                if (AllServices.TryGetValue(type, out IService service))
                {
                    m_servicesLateUpdateOrder.AddLast(service);
                }
            }
        }

        public void BindWithInstance<T>(IService serviceInstance) where T : class, IService
        {
            if (!CheckTypeKey<T>(serviceInstance))
            {
                return;
            }
            m_allServices.Add(typeof(T), serviceInstance);
            if (serviceInstance is MonoBehaviour monoBehaviour && monoBehaviour.transform != m_transform)
            {
                monoBehaviour.transform.parent = m_transform;
            }
            DebugLogger.Info(this, $"{typeof(T).Name} has been binded with a simple instance.");
        }

        public void BindWithPrefab<T>(BaseService servicePrefab) where T : class, IService
        {
            if (!CheckTypeKey<T>(servicePrefab))
            {
                return;
            }
            BaseService serviceInstance = GameObject.Instantiate(servicePrefab, m_transform);
            m_allServices.Add(typeof(T), serviceInstance);
            DebugLogger.Info(this, $"{typeof(T).Name} has been binded with a prefab instance.");
        }

        public void BindWithMockService<T>() where T : class, IService
        {
            if (m_allServices.ContainsKey(typeof(T)))
            {
                DebugLogger.Error(this, $"The binding container already references the {typeof(T).Name} type.");
                return;
            }
            T mockService = Substitute.For<T>();
            m_allServices.Add(typeof(T), mockService);
            DebugLogger.Info(this, $"{typeof(T).Name} has been binded with a mock instance.");
        }

        private bool CheckTypeKey<T>(IService serviceInstance) where T : class, IService
        {
            if (serviceInstance is null)
            {
                BindWithMockService<T>();
                return false;
            }
            if (serviceInstance is not T)
            {
                DebugLogger.Error(this, $"The object type you gave is not a service.");
                return false;
            }
            if (m_allServices.ContainsKey(typeof(T)))
            {
                DebugLogger.Error(this, $"The binding container already references the {typeof(T).Name} type.");
                return false;
            }
            return true;
        }
    }
}
