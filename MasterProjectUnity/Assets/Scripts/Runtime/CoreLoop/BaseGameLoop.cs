using MasterProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject
{
    public abstract class BaseGameLoop : MonoBehaviour
    {
        protected BindingContainer m_container;

        protected abstract IReadOnlyList<Type> InitializeServicesOrder { get; }
        protected abstract IReadOnlyList<Type> UpdateServicesOrder { get; }
        protected abstract IReadOnlyList<Type> LateUpdateServicesOrder { get; }

        protected virtual void Awake()
        {
            InstantiateContainer();
            InstantiateServices(m_container);
            SetupServicesContainers();
            GenerateScenes();
            SetupServicesDependencies();
            SetupOtherDependencies();
            InitializeServices();
        }

        protected virtual void InstantiateContainer()
        {
            m_container = new BindingContainer(transform);
        }

        protected abstract void InstantiateServices(in BindingContainer container);

        protected virtual void SetupServicesContainers()
        {
            m_container.SetupOrders(UpdateServicesOrder, LateUpdateServicesOrder);
        }

        protected virtual void SetupServicesDependencies()
        {
            foreach (KeyValuePair<Type, IService> kvp in m_container.AllServices)
            {
                InjectionUtilities.InjectDependencies(typeof(ServiceDepencency), m_container.AllServices, kvp.Value);
            }
        }

        protected abstract void SetupOtherDependencies();

        // A REVOIR
        protected virtual void GenerateScenes()
        {
        }

        protected virtual void InitializeServices()
        {
            foreach (Type type in InitializeServicesOrder)
            {
                if (m_container.AllServices.TryGetValue(type, out IService service))
                {
                    service.Initialize();
                }
            }
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            IEnumerator<IService> currentService = m_container.ServicesUpdateOrder.GetEnumerator();
            while (currentService.MoveNext())
            {
                currentService.Current.BaseUpdate(deltaTime);
            }
        }

        protected virtual void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            IEnumerator<IService> currentService = m_container.ServicesLateUpdateOrder.GetEnumerator();
            while (currentService.MoveNext())
            {
                currentService.Current.BaseLateUpdate(deltaTime);
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (Type type in InitializeServicesOrder)
            {
                if (m_container.AllServices.TryGetValue(type, out IService service))
                {
                    service.Unload(); // CHECK IF SERVICES ARE UNLOADED BEFORE
                }
            }
        }
    }
}
