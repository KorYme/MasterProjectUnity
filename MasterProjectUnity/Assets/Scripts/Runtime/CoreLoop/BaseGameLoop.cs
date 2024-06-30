using MasterProject.Services;
using MasterProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject
{
    public abstract class BaseGameLoop : MonoBehaviour
    {
        protected Dictionary<Type, BaseServices> m_allServices;
        protected LinkedList<BaseServices> m_servicesUpdateOrder;

        protected virtual void Awake()
        {
            InstantiateContainers();
            InstallBindings();
            SetupServicesContainers();
            GenerateScenes();
            SetupServicesDependencies();
            InitializeServices();
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            LinkedList<BaseServices>.Enumerator currentService = m_servicesUpdateOrder.GetEnumerator();
            while (currentService.MoveNext())
            {
                currentService.Current.Update(deltaTime);
            }
        }

        protected virtual void InstantiateContainers()
        {
            m_allServices = new Dictionary<Type, BaseServices>();
            m_servicesUpdateOrder = new LinkedList<BaseServices>();
        }

        // REWORK AVEC PREFAB
        protected abstract void InstallBindings();

        protected virtual void SetupServicesContainers()
        {
            foreach (Type type in ServicesLoopOrder.UpdateServicesOrder)
            {
                if (m_allServices.TryGetValue(type, out BaseServices manager))
                {
                    m_servicesUpdateOrder.AddLast(manager);
                }
            }
        }

        protected virtual void SetupServicesDependencies()
        {
            foreach (KeyValuePair<Type, BaseServices> kvp in m_allServices)
            {
                InjectionUtilities.InjectDependencies(kvp.Value, typeof(ServiceDepencency), m_allServices);
            }
        }

        protected virtual void GenerateScenes()
        {
        }

        protected virtual void InitializeServices()
        {
            foreach (Type type in ServicesLoopOrder.InitializeServicesOrder)
            {
                if (m_allServices.TryGetValue(type, out BaseServices service))
                {
                    service.Initialize();
                }
            }
        }
    }
}
